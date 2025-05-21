using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class CVService:GenericRepo<CV>,ICVService
    {
        private readonly ApplicationDbContext _context ;
        public CVService(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
