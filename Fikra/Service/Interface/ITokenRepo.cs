using SparkLink.Models.Identity;

namespace SparkLink.Service.Interface
{
    public interface ITokenRepo
    {
        public Task<string> GenerateToken(IList<string> Roles, ApplicationUser  user);
    }
}
