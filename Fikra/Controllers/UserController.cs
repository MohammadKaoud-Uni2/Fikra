using AutoMapper;
using Fikra.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using System.Security.Claims;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServices _identityService;
        private readonly IMapper _Mapper;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor  _httpcontextAccessor;
        private readonly IEmailService _emailService;
        public UserController(UserManager<ApplicationUser>userManager,IIdentityServices identityServices,IMapper mapper,IPhotoService photoService,IHttpContextAccessor httpContextAccessor,IEmailService emailService)
        {
            _userManager = userManager;
            _identityService= identityServices;
            _Mapper = mapper;
            _photoService= photoService;
            _httpcontextAccessor = httpContextAccessor;
            _emailService= emailService;
            
        }
        [HttpGet]
        [Route("GetPersonalInformation")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> GetPersonalInformation()
        {
            var user = _httpcontextAccessor.HttpContext.User;
            string UserId = "";
            if (user.Identity.IsAuthenticated)
            {
                UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
            }
            var userTofind=await _userManager.FindByIdAsync(UserId);

            var userAfterMapping= _Mapper.Map<GetProfileDto>(userTofind);
            if (userAfterMapping != null)
            {
                return Ok(userAfterMapping);
            }
            return BadRequest("There Was A problem While Mapping ");
        }
        [HttpPut]
        [Route("UpdatePersonalInfo")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> UpdatePersonalInformation([FromForm]UpdateUserDto updateUserDto, [FromForm] IFormFile? profilePicture)
        {
            var currentUsername=await _identityService.GetCurrentUserName();
            var user = await _userManager.FindByNameAsync(currentUsername);
            user.Email=updateUserDto.Email;
            user.LinkedinUrl=updateUserDto.LinkedInUrl;
            if (profilePicture != null)
            {
                var profilePictureUrl = await _photoService.UploadPhoto(profilePicture);
                user.ImageProfileUrl =profilePictureUrl;
            }
            user.CompanyName=updateUserDto.CompanyName;
            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Update failed", errors });
            }
            return Ok(result);

        }
        //[HttpPost]
        //[Route("TestHomsi")]
        //public async Task<IActionResult> testHomsi()
        //{
        //    var resultofSendingEmail = await _emailService.SendEmail("mohammadkaoud17@gmail.com", "Welcome from our company Sparklink", "Say hello and testing");
        //    if (resultofSendingEmail == "Success")
        //    {
        //        return Ok("CheckYourMailBox");
        //    }
        //    return BadRequest("Error");
        //}
    }
}
