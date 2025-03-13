namespace SparkLink.Service.Interface
{
    public interface IAuthenticationService
    {
       public  Task<string> ConfirmEmail(string UserId,string Code);
        public Task<string> ResetPassword(string Email);
        public Task<string >ResetPasswordOperation(string Email,string Code,string Password);
    }
}
