using AutoMapper;
using Fikra.Hubs;
using Fikra.Models;
using Fikra.Service.Implementation;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using Stripe;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobRequestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdeaService _ideaService;
        private readonly IJoinRequestService _joinRequestService;
        private readonly PresenceTracker _presenceTracker;
        private readonly IIdentityServices _identityServices;
        private readonly ICVService _cvService;
        private readonly IHubContext<GroupChatHub> _hubContext;
        private readonly IMapper _mapper;
        private readonly IPresistanceGroupService _presistanceGroupService;

        public JobRequestController(UserManager<ApplicationUser> userManager, IIdeaService IdeaService, IJoinRequestService joinRequestService, PresenceTracker presenceTracker, IIdentityServices identityServices, ICVService cVService, IHubContext<GroupChatHub> contractHub, IMapper mapper, IPresistanceGroupService presistanceGroupService)
        {
            _userManager = userManager;
            _ideaService = IdeaService;
            _joinRequestService = joinRequestService;
            _presenceTracker = presenceTracker;
            _identityServices = identityServices;
            _cvService = cVService;
            _hubContext = contractHub;
            _mapper = mapper;
            _presistanceGroupService = presistanceGroupService;

        }
        [HttpPost]
        [Route("SendingJobRequest")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> SendJobRequestToOwner([FromBody] SendingJobRequestDto sendingJobRequestDto)
        {
            var currentUserName = await _identityServices.GetCurrentUserName();
            var freelancer = await _userManager.FindByNameAsync(currentUserName);
            var ideas = await _ideaService.GetTableAsNoTracking().ToListAsync();
            if (ideas == null)
            {
                return BadRequest("There is No Ideas In DB");
            }
            var idea = ideas.FirstOrDefault(x => x.Title == sendingJobRequestDto.IdeaTitle);
            if (idea == null)
            {
                return BadRequest("There are no Ideas With this Title");
            }

            var cvs = await _cvService.GetTableAsNoTracking().Include(x => x.Technologies).ToListAsync();
            if (!cvs.Any())
            {
                return BadRequest("There are no CVs In Db");
            }

            var cvForSpecificUser = cvs.FirstOrDefault(x => x.ApplicationUserId == freelancer.Id);



            if (cvForSpecificUser == null)
            {
                return BadRequest("No CV found for the freelancer.");
            }
            var jobrequestVerification = await _joinRequestService.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.ideaTitle == sendingJobRequestDto.IdeaTitle && x.FreelancerId == freelancer.Id);
            if (jobrequestVerification != null)
            {
                return BadRequest("You have been sent joint Request With same Data Request Before");
            }


            var technologyList = cvForSpecificUser.Technologies
                .Select(t => $"{t.Technology} (Level: {t.Level})")
                .ToList();

            var technologiesMessage = string.Join(", ", technologyList);

            var message = $"Hello, I am {freelancer.UserName}. I am experienced in {technologiesMessage}. I am ready to implement your project,\n Check My cv from here {cvForSpecificUser.CVPdfUrl} ";

            var jobRequest = new JoinRequest
            {
                CvUrl = cvForSpecificUser.CVPdfUrl,
                FreelancerId = freelancer.Id,
                ideaTitle = sendingJobRequestDto.IdeaTitle,
                Message = message,
                SentAt = DateTime.Now,
                Status = "Pending",
                IdeaOwnerName = idea.IdeaOwnerName,
            };

            await _joinRequestService.AddAsync(jobRequest);
            await _joinRequestService.SaveChangesAsync();

            var ideaOwnerConnectionId = await _presenceTracker.GetConnectionIdsForUser(idea.IdeaOwnerName);
            if (ideaOwnerConnectionId != null && ideaOwnerConnectionId.Any())
            {

                await _hubContext.Clients.Client(ideaOwnerConnectionId.First()).SendAsync("SendJobRequest", message);
            }


            return Ok();
        }
        [HttpGet]
        [Route("GetJobRequests")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetJobRequest()
        {
            var userName = await _identityServices.GetCurrentUserName();
            var user = await _userManager.FindByNameAsync(userName);
            List<ReceiveJobRequest> receiveRequests = new List<ReceiveJobRequest>();
            var JointRequest = await _joinRequestService.GetTableAsNoTracking().Where(x => x.IdeaOwnerName == userName && x.Status == "Pending").ToListAsync();
            if (JointRequest.Any())
            {
                receiveRequests = _mapper.Map<List<ReceiveJobRequest>>(JointRequest);
                return Ok(receiveRequests);
            }
            return Ok (new List<ReceiveJobRequest>());  
            

        }
        [HttpPost]
        [Route("RejectRequest")]
        [Authorize(AuthenticationSchemes ="Bearer")]

        public async Task<IActionResult> RejectJobRequest([FromQuery] int  jobRequestId)
        {
            var jobRequest = await _joinRequestService.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.Id==jobRequestId);
           if(jobRequest == null)
            {
                return NotFound();
            }
            jobRequest.Status = "Rejected";
          await  _joinRequestService.UpdateAsync(jobRequest);
            await _joinRequestService.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("accept")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AcceptJobRequest([FromBody] AcceptDTO acceptDTO)
        {
            var CurrentUserName = await _identityServices.GetCurrentUserName();
            var CurrentUser = await _userManager.FindByNameAsync(CurrentUserName);

            if (CurrentUser == null)

            {
                return Unauthorized("User not authenticated.");
            }
            var freelancer = await _userManager.FindByNameAsync(acceptDTO.RequesterUserName);
            if (freelancer == null)
            {
                return NotFound($"Job Requester With Such name {acceptDTO.RequesterUserName} Not found");
            }
            var jointRequest = await _joinRequestService.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.ideaTitle == acceptDTO.IdeaTitle && x.FreelancerId == freelancer.Id);
            jointRequest.Status = "Accepted";
            await _joinRequestService.UpdateAsync(jointRequest);
            await _joinRequestService.SaveChangesAsync();

            var group = await _presistanceGroupService.CreateGroupAsync(acceptDTO.IdeaTitle, CurrentUser.Id);


            var user = await _userManager.FindByNameAsync(acceptDTO.RequesterUserName);
            await _presistanceGroupService.AddMemberToGroupAsync(group.Id, user.Id);


            var connections = await _presenceTracker.GetConnectionIdsForUser(acceptDTO.RequesterUserName);
            if (connections != null)
            {
                foreach (var connectionId in connections)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("JobAccepted", new
                    {
                        GroupId = group.Id,
                        GroupName = group.Name,
                        IdeaTitle = acceptDTO.IdeaTitle

                    });
                }
            }

            return Ok(new
            {
                Message = "Job request accepted.",
                GroupId = group.Id,
                Notified = connections?.Count > 0
            });
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("GetGroups")]

        public async Task<IActionResult> GetGroups()
        {
            var currentUserName = await _identityServices.GetCurrentUserName();
            var currentUser=await _userManager.FindByNameAsync(currentUserName);
            var groups = await _presistanceGroupService.GetGroupChatRelatedToUser(currentUser.Id);
            
            if(groups != null) {
            var groupsDto=_mapper.Map<List<getChatGroupDto>>(groups);
            
                foreach (var group in groupsDto)
                {
                    
                        var userNames = (await _presistanceGroupService.GetGroupMembersAsync(group.Id))
                            .Select(x => x.UserName)
                            .ToList();

                        group.memebers = userNames;
                    

                }
               return Ok(groupsDto);
            }
            return Ok(new List<getChatGroupDto>());
            
        }

        

    

    }

    public class SendingJobRequestDto
    {
        public string IdeaTitle { get; set; }
    }
    public class ReceiveJobRequest
    {
        public int  Id { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public string ideaTitle { get; set; }
    }
    public class AcceptDTO
    {
        public string IdeaTitle { get; set; }
        public string RequesterUserName { get; set; }
    }
}
