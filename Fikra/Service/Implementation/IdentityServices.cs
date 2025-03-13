using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using System.Security.Principal;

namespace SparkLink.Service.Implementation
{
    public class IdentityServices : IIdentityServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public IdentityServices(UserManager<ApplicationUser>userManager,SignInManager<ApplicationUser>SignInManager)
        {
            _userManager = userManager;
            _signInManager=SignInManager;           
            
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
    }
}
