using Fikra.Models;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SparkLink.Data;
using SparkLink.Models.Identity;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class RequestRepo : GenericRepo<Request>, IRequestRepo

    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRSAService _rsaService;

        public RequestRepo(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IRSAService IrsService) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _rsaService = IrsService;
        }

        public async Task<Request> GetRequestBetweenTwoUsers(string IdeaOwnerUserName, string InvestorUserName)
        {
            var result = await _context.Requests.AsNoTracking().FirstOrDefaultAsync(x => x.IdeaOwnerName == IdeaOwnerUserName && x.InvestorName == InvestorUserName);
            return result;
        }
        public async Task UpdateRequest(Request request)
        {
            request.Status = "Accepted";
            _context.Requests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Request>> GetRequestByUserName(string UserName)
        {
           var requests= await _context.Requests.AsNoTracking().Where(x=>x.IdeaOwnerName==UserName && x.Status=="Pending").ToListAsync();
            if (requests != null)
            {
                return requests;
            }
            return null;

        }

        public async Task RejectRequest(Request request)
        {
            request.Status = "Rejected";
            _context.Requests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}

