using Fikra.Models;
using Fikra.Models.Dto;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Identity;
using SparkLink.Data;
using SparkLink.Models.Identity;
using SparkLink.Service.Implementation;
using SparkLink.Service.Interface;

namespace Fikra.Service.Implementation
{
    public class ComplaintService:GenericRepo<Complaint>,IComplaintService
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdentityServices _IdentityServices;
        private readonly UserManager<ApplicationUser> _userManager;
        public ComplaintService(ApplicationDbContext context,IIdentityServices identityServices,UserManager<ApplicationUser>userManager):base(context) 
        {
            _context = context;
            _IdentityServices = identityServices;
            _userManager = userManager;
            

        }
       
    }
}
