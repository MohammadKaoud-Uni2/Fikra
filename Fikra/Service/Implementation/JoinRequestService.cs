using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class JoinRequestService:GenericRepo<JoinRequest>,IJoinRequestService
    {
        private readonly ApplicationDbContext _context;
        public JoinRequestService(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
