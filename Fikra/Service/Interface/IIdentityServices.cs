using Microsoft.Identity.Client;
using SparkLink.Models.Identity;

namespace SparkLink.Service.Interface
{
    public interface IIdentityServices
    {
        public  Task<ApplicationUser> FindUserById(string Id);
        public  Task<ApplicationUser>FindUserByName (string Name);
        public  Task<ApplicationUser>FindUserByEmail(string Email);   
        public Task<bool>CheckifTheUseriSActive(ApplicationUser user);
        public Task<string>GetCurrentUserName();
        public Task<string> GetCurrentId();

    }
}
