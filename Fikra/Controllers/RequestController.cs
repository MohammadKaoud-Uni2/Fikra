using AutoMapper;
using Fikra.Models.Dto;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using Stripe;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRequestRepo _requestRepo;
        private readonly IIdentityServices _IdentityService;
        private readonly IMapper _mapper;
        private readonly IRSAService _rsaService;
        public RequestController(UserManager<ApplicationUser>userManager,IRequestRepo requestRepo,IIdentityServices identityServices,IMapper mapper,IRSAService IrsaService)
        {
            _userManager = userManager;
            _requestRepo = requestRepo;
            _IdentityService = identityServices;
            _mapper = mapper;
            _rsaService=IrsaService;
        }
        [HttpGet]
        [Route("GetRequests")]
        [Authorize(AuthenticationSchemes ="Bearer",Roles ="IdeaOwner")]
        public async Task<IActionResult> GetIdeaOwnerRequests()
        {
            var ideaOwnerName= await _IdentityService.GetCurrentUserName();
            var currentUser=await _userManager.FindByNameAsync(ideaOwnerName);


          var requests=await  _requestRepo.GetRequestByUserName(currentUser.UserName);
            if (requests == null)
            {
                return Ok();
            }
         var  requestsAfterMapping= _mapper.Map<List<GetRequestDto>>(requests);
            foreach (var request in requestsAfterMapping)
            {
               request.RequestDetail= _rsaService.DecryptData(request.RequestDetail);
            }

            return Ok(requestsAfterMapping);
        }
        [HttpGet]
        [Route("GetRequestsCount")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "IdeaOwner")]
        public async Task<IActionResult> GetIdeaOwnerRequestsCount()
        {
            var ideaOwnerName = await _IdentityService.GetCurrentUserName();
            var ideaOwner =await  _userManager.FindByNameAsync(ideaOwnerName);
            var request = await _requestRepo.GetRequestByUserName(ideaOwnerName);
            if (request == null)
            {
                return Ok();
            }
            return Ok(new
            {
                RequestCount = request.Count,
            });
        }
        [HttpGet]
        [Route("GetRequestRelatedToInvestor")]
        [Authorize(AuthenticationSchemes ="Bearer",Roles ="Investor")]
        public async Task<IActionResult> GetRequestRelatedToInvestor()
        {
            var InvestorRequests=new List<GetRequestInvestorDto>();
            var investorname=await _IdentityService.GetCurrentUserName();
            var requests = await _requestRepo.GetTableAsNoTracking().Where(x=>x.Status=="Accepted"&& x.InvestorName==investorname).ToListAsync();
            if (requests.Any())
            {
                foreach (var request in requests)
                {
                    var investorRequest = new GetRequestInvestorDto()
                    {
                        IdeaOwnerName = request.IdeaOwnerName,
                        IdeaTitle = request.IdeaTitle,
                    };

                    InvestorRequests.Add(investorRequest);
                }
                return Ok(InvestorRequests);
            }
            return Ok();
        }


    }
    public class GetRequestInvestorDto
    {
        public string IdeaTitle { get; set; }
        public string IdeaOwnerName { get; set; }
    }
}
