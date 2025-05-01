using AutoMapper;
using Fikra.Models.Dto;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public RequestController(UserManager<ApplicationUser>userManager,IRequestRepo requestRepo,IIdentityServices identityServices,IMapper mapper)
        {
            _userManager = userManager;
            _requestRepo = requestRepo;
            _IdentityService = identityServices;
            _mapper = mapper;
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
            return Ok(requestsAfterMapping);
        }
        [HttpGet]
        [Route("GetRequestsCount")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "IdeaOwner")]
        public async Task<IActionResult> GetIdeaOwnerRequestsCount()
        {
            var ideaOwnerName = await _IdentityService.GetCurrentUserName();
            var ideaOwner = _userManager.FindByNameAsync(ideaOwnerName);
            var request = await _requestRepo.GetRequestByUserName(ideaOwnerName);
            if (request == null)
            {
                return Ok();
            }
            return Ok(request.Count);
        }

    }
}
