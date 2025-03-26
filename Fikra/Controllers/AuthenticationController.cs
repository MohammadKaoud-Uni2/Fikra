
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using SparkLink.Helper;
using SparkLink.Models.Dto;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using System.Runtime.CompilerServices;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IIdentityServices identityServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenRepo _tokenRepo;
        private readonly IEmailService _emailService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPhotoService _photoService;
        private readonly IPdfService _IpdfService;
        
        public AuthenticationController(IIdentityServices IdentityServices, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenRepo tokenRepo, IEmailService emailService, IAuthenticationService authenticationService,IPhotoService photoService,IPdfService pdfService)
        {
            this.identityServices = IdentityServices;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenRepo = tokenRepo;
            _emailService = emailService;
            _authenticationService = authenticationService;
            _photoService = photoService;
            _IpdfService = pdfService;
        }
        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register([FromForm] RegisterDto RegisterDto ,[FromForm]IFormFile profilePicture)
        {
            ApplicationUser userToFind = await identityServices.FindUserByName(RegisterDto.FirstName + RegisterDto.LastName);
            userToFind = await identityServices.FindUserByEmail(RegisterDto.Email);
            if (userToFind != null)
            {
                return BadRequest("You Have Been Registered Before");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("There Was  A problem Validate One of Columns ");

            }
            var roleName = "";
            switch (RegisterDto.RoleRequestd)
            {
                case "IdeaOwner":

                    roleName = "IdeaOwner";
                    break;

                case "Freelancer":
                    roleName = "Freelancer";
                    break;
                case "Investor":
                    roleName = "Investor";
                    break;

                default:

                    break;

            }
          
            var NewUser = new ApplicationUser
            {
                Email = RegisterDto.Email,
                UserName = RegisterDto.FirstName + RegisterDto.LastName,
                Gender = RegisterDto.Gender,
                LinkedinUrl = RegisterDto.LinkedinUrl,
                FatherName = RegisterDto.FatherName,
                Country = RegisterDto.Country,
                FirstName = RegisterDto.FirstName,
                LastName = RegisterDto.LastName,
                CompanyName = RegisterDto.CompanyName,



            };
            var resultofCreatingUser = await _userManager.CreateAsync(NewUser, RegisterDto.Password);
            string imageUrl = "";
            if (resultofCreatingUser.Succeeded)
            {
                if (profilePicture != null)
                {
                  imageUrl=await  _photoService.UploadPhoto(profilePicture);
                }
                NewUser.ImageProfileUrl= imageUrl;

                var resultofAssignUserToRole = await _userManager.AddToRoleAsync(NewUser, roleName);

                if (resultofAssignUserToRole.Succeeded)
                {
                    var emailVerificationResult = await _emailService.SendEmail(NewUser.Email, $"Welcome {NewUser.UserName} This Email for Verification your Account ", "Person Verification");
                    if (emailVerificationResult == "Success")
                    {


                        NewUser.EmailConfirmed = true;
                        await _userManager.UpdateAsync(NewUser);
                        return Ok("User Have Been Registered Successfully check your Email Address");
                    }
                    return BadRequest("User Have Been Registered But There Was A problem While Sending The Email");
                }
                return BadRequest("There Was  A problem While Assign Role To user");
            }
            return BadRequest("There Was A problwem While Creating the User ");

        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await identityServices.FindUserByEmail(loginDto.Email);
            if (user != null)
            {
                var resultofCheckingPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (resultofCheckingPassword)
                {
                    var rolesAssignToUser = await _userManager.GetRolesAsync(user);
                    var JwtToken = await _tokenRepo.GenerateToken(rolesAssignToUser, user);
                    var response = new LoginResponseDto()
                    {
                        Token = JwtToken,
                        UserName = user.UserName,
                    };
                    if (response != null)
                    {
                        var emailSendingResult = await _emailService.SendEmail(user.Email, "Welcome Prince", "Say Hello from our Company ");

                        return Ok(response);
                    }

                }
                return BadRequest("there Was A problem While Loggin Becuase Wrong password");
            }
            return BadRequest("User Not Found ");

        }
        [HttpPost]
        [Route("ResetPasswordRequest")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var resultofRequest = await _authenticationService.ResetPassword(resetPasswordRequestDto.Email);
            switch (resultofRequest)
            {
                case "CodeHasBeenSentSuccessfully":
                    return Ok("Check your Mail Box");



                case "Problem When Updating the User":
                    return BadRequest("There Was A Problem While Updating User Password Could be from the intenral Server !");

                case "UserNotFound":
                    return BadRequest("User Not Found");
                default:
                    return BadRequest(" The Problem Could be the from Internal Server Error");

            }

        }
        [HttpGet]
        [Route("ResetPasswordOperation")]
        public async Task<IActionResult> ResetPasswordOperation([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var resultofRequest = await _authenticationService.ResetPasswordOperation(resetPasswordDto.Email, resetPasswordDto.Code, resetPasswordDto.Password);
            switch (resultofRequest)
            {
                case "Success":
                    return Ok("Password Has Been Reseted Successfully");



                case "faild While Updating the new Password":
                    return BadRequest("faild While Updating the new Password !");

                case "Faild While Removing The Old Password":
                    return BadRequest("Faild While Removing The Old Password");
                case "Code Is Not Equal":
                    return BadRequest("Code Written is Not Matching The Code Sent");
                case "User Not Found":
                    return BadRequest("User Not Found");
                default:
                    return BadRequest(" The Problem Could be the from Internal Server Error");


            }

        }
        [Route("GeneratePdf")]
        [HttpGet]
        //[Authorize("Bearer")]
        public async Task<IActionResult> GeneratePdf()
        {
            // var IdeaOwner = new ApplicationUser();
            //var Investor = new ApplicationUser();

            //var secondUser = await _userManager.FindByIdAsync(userId);
            //var secondUserRoles = await _userManager.GetRolesAsync(secondUser);
            //if (secondUserRoles.Contains("IdeaOwner"))
            //{
            //    IdeaOwner = secondUser;
            //}


            var result = await _IpdfService.ReciveImage();
            var pdfUrl = await _IpdfService.GenerateContract("mohamad", "Rami", 1221221.1m, DateTime.Now, "mohammadAhmadKaoud", "RamiNabeelYehay", result);
            if (pdfUrl != null)
            {
                return Ok(pdfUrl);
            }
            return BadRequest("There Was A problem While Accessing the Pdf");
        }

    }
}

