using AutoMapper;
using Fikra.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServices _identityService;
        private readonly IMapper _Mapper;
        public UserController(UserManager<ApplicationUser>userManager,IIdentityServices identityServices,IMapper mapper)
        {
            _userManager = userManager;
            _identityService= identityServices;
            _Mapper = mapper;
            
        }
        [HttpGet]
        [Route("GetPersonalInformation")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> GetPersonalInformation()
        {
            var currentUserName=await _identityService.GetCurrentUserName();
            var user=await _userManager.FindByNameAsync(currentUserName);

            var userAfterMapping= _Mapper.Map<GetProfileDto>(user);
            if (userAfterMapping != null)
            {
                return Ok(userAfterMapping);
            }
            return BadRequest("There Was A problem While Mapping ");
        }
        [HttpPut]
        [Route("UpdatePersonalInfo")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> UpdatePersonalInformation([FromBody]UpdateUserDto updateUserDto )
        {
            var currentUsername=await _identityService.GetCurrentUserName();
           var user=await  _userManager.FindByNameAsync(currentUsername);
            user.Email=updateUserDto.Email;
            user.PhoneNumber=updateUserDto.PhoneNumber;
            user.UserName=updateUserDto.UserName;
            user.LinkedinUrl=updateUserDto.LinkedInUrl;
            user.ImageProfileUrl=updateUserDto.ImageProfileUrl;
            user.CompanyName=updateUserDto.CompanyName;
            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Update failed", errors });
            }
            return Ok(result);

        }
    }
}
