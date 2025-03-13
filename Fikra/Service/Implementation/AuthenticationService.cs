
using Microsoft.AspNetCore.Identity;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;

namespace SparkLink.Service.Implementation
{
    public class AuthenticationService:IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        public AuthenticationService(UserManager<ApplicationUser> userManager,IEmailService emailService)
        {
            _emailService = emailService;
            _userManager = userManager;
        }
        public async Task<string> ConfirmEmail(string UserId, string Code)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null || Code == null)
            {
                return "userNotFound Or Code Field is Empty";
            }
            var result = await _userManager.ConfirmEmailAsync(user, Code);
            if (result.Succeeded)
            {
                return "Succeeded";
            }
            return "Wrong Or Incorrect Validation To Email Service";
        }

        public async Task<string> ResetPassword(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                var CodeToGenerate = new Random();
                var CodeToPlace = CodeToGenerate.Next(1000000);

                user.Code = CodeToPlace.ToString();
                var resultofUpdate = await _userManager.UpdateAsync(user);
                if (resultofUpdate.Succeeded)
                {
                    var result = await _emailService.SendEmail(Email, $"Your Code Please Put it To Reset your password :{CodeToPlace}", "Reset Password");
                    if (result == "Success")
                    {
                        return "CodeHasBeenSentSuccessfully";
                    }
                }
                return "Problem When Updating the User";

            }
            return "UserNotFound";
        }

        public async Task<string> ResetPasswordOperation(string Email, string Code, string Password)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                if (user.Code == Code)
                {
                    var resultOfDeletingOldPassword = await _userManager.RemovePasswordAsync(user);
                    if (resultOfDeletingOldPassword.Succeeded)
                    {
                        var resultofUpdatingPassword = await _userManager.AddPasswordAsync(user, Password);
                        if (resultofUpdatingPassword.Succeeded)
                        {
                            return "Success";
                        }
                        return "faild While Updating the new Password";
                    }
                    return "Faild While Remove The Old Password";

                }
                return "Code Is Not Equal";

            }
            return "User Not Found";



        }
    }
}
