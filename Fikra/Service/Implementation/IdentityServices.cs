using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using System.Security.Claims;
using System.Security.Principal;

namespace SparkLink.Service.Implementation
{
    public class IdentityServices : IIdentityServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;   
        public IdentityServices(UserManager<ApplicationUser>userManager,SignInManager<ApplicationUser>SignInManager,IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager=SignInManager;          
            _httpContextAccessor = httpContextAccessor;
            
        }

        public async  Task<bool> CheckifTheUseriSActive(ApplicationUser user)
        {
            if (user.IsActive)
            {
                return true;
            }
            return false;
        }

        public async Task<ApplicationUser> FindUserByEmail(string Email)
        {
            if (Email != null)
            {
                var user=await _userManager.FindByEmailAsync(Email);
                if(user != null)
                {
                    return user;
                }
                return null;


            }
            return null;
        }

        public async Task<ApplicationUser> FindUserById(string Id)
        {
            if (Id != null)
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user != null)
                {
                    return user;
                }
                return null;


            }
            return null;

        }

        public  async Task<ApplicationUser> FindUserByName(string Name)
        {
            if (Name != null)
            {
                var user = await _userManager.FindByNameAsync(Name);
                if (user != null)
                {
                    return user;
                }
                return null;


            }
            return null;
        }
       
        

        public async Task<string> GetCurrentUserName()
        {
            var currentUserName = _httpContextAccessor.HttpContext.User.Identity.Name;
            return currentUserName;
        }

        async Task <ApplicationUser> IIdentityServices.FindUserByEmail(string Email)
        {
            var user=await _userManager.FindByEmailAsync(Email);
            return user;
        }

        Task<ApplicationUser> IIdentityServices.FindUserById(string Id)
        {
            throw new NotImplementedException();

        }

        Task<ApplicationUser> IIdentityServices.FindUserByName(string Name)
        {
            throw new NotImplementedException();
        }

        async Task<string> IIdentityServices.GetCurrentId()
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return userId;
            }
            return null;
        }

      
    }
}
