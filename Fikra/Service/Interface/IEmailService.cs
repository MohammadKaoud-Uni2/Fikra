namespace SparkLink.Service.Interface
{
    public interface IEmailService
    {
         public Task<string> SendEmail(string Email, string Message, string? reason);
    }
}
