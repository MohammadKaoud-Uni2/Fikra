using AutoMapper;
using Fikra.Models;
using Fikra.Models.Dto;
using Fikra.Service.Implementation;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaintService _complaintService;
       private readonly IIdentityServices _IdentityService;
        private readonly IPhotoService _photoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPenaltyPointService _penaltyPointService;
        private readonly IMapper _mapper;
        public ComplaintController(IComplaintService complaintService,IIdentityServices identityServices,UserManager<ApplicationUser>userManager,IPhotoService photoService,IMapper mapper,IPenaltyPointService penaltyPointService)
        {
            _complaintService = complaintService;

            _IdentityService = identityServices;
            _userManager = userManager;
            _photoService = photoService;
            _mapper = mapper;
            _penaltyPointService = penaltyPointService;
            
        }
        [HttpPost]
        [Route("SubmitComplaint")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> SubmitComplaint([FromForm] SubmitComplaintRequest submitComplaintRequest, [FromForm] IFormFile ?Evidence )
        {
            var currentUserName = await _IdentityService.GetCurrentUserName();
            var currentUser = await _userManager.FindByNameAsync(currentUserName);
            string ComplaintUrl = "";
            
            var complaint = new Complaint
            {
                FromUserId = currentUser.Id,
                AgainstUserName = submitComplaintRequest.AgainstUserName,
                Reason = submitComplaintRequest.Reason.ToString(),
                
                
                CreatedAt = DateTime.UtcNow,
                Status = ComplaintStatus.Pending
            };
            if (submitComplaintRequest.AdditionalNotes != null)
            {
              complaint.AdditonalNote=submitComplaintRequest.AdditionalNotes;
            }
            if (Evidence != null)
            {
                ComplaintUrl=await _photoService.UploadComplaint(Evidence);
            }

            complaint.EvidenceUrl = ComplaintUrl; 

            await _complaintService.AddAsync(complaint);
            await _complaintService.SaveChangesAsync();
            return Ok(new
            {
                message = "Complaint has Been sent to Admin  Wait for Response :)"
            });
        }
        [Route("GetAllComplaints")]
        //[Authorize(AuthenticationSchemes ="Bearer",Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllComplaints()
        {
            var allcomplaints = await _complaintService.GetTableAsNoTracking().Where(x => x.Status == ComplaintStatus.Pending).ToListAsync();
            if (allcomplaints.Any())
            {
               var complaintsaftermapping=_mapper.Map<List<ReviewComplaintRequest>>(allcomplaints);
                return Ok(complaintsaftermapping);  
            }
            return Ok(new
            {
                message="there are no Current complaints"
            });

        }
        [HttpPost("ReviewComplaint")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> ReviewComplaint([FromBody] DetermineComplaintDto request)
        {
            var complaints = await _complaintService.GetTableAsNoTracking().ToListAsync();
            var complaint =  complaints.FirstOrDefault(x => x.Id == request.ComplaintId);
            if (complaint == null)
                return NotFound("Complaint not found");

            if (complaint.Status != ComplaintStatus.Pending)
                return BadRequest("Complaint already reviewed");
            if (request.Points > 0)
            {
                complaint.Status = ComplaintStatus.Accepted;
                complaint.ReviewedAt = DateTime.UtcNow;
            }

           await  _complaintService.UpdateAsync(complaint);
                if (request.Points ==null || request.Points <= 0)
                    return BadRequest("Penalty points must be greater than 0");

                var user = await _userManager.FindByNameAsync(complaint.AgainstUserName);
                if (user == null)
                    return NotFound("User not found");

                var penalty = new PenaltyPoint
                {
                  
                    ApplicationUserId = user.Id,
                    ApplicationUser= user,
                    IssuedAt = DateTime.UtcNow,
                    Reason = complaint.Reason.ToString(),
                    ComplaintId = complaint.Id,
                    Points=request.Points,
                };

                await _penaltyPointService.AddAsync(penalty);

              
                int totalPoints = await _penaltyPointService.GetPointCountAsync(user.Id);

                if (totalPoints >= 10)
                {
                 user.IsActive = false;

                  
                }
            await _userManager.UpdateAsync(user);
            await _complaintService.UpdateAsync(complaint);
           
            await _complaintService.SaveChangesAsync();
            return Ok(new
            {
                message = "Complaint Has been Reviewed Succesfully"
            });
        }

         
        }

    
    public class DetermineComplaintDto
    {
        public string ComplaintId { get; set; }
        public int Points { get; set; }
        
    }

}
