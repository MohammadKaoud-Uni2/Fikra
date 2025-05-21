using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;
using SparkLink.Service.Interface;

namespace Fikra.Service.Implementation
{
    public class PenaltyPointService:GenericRepo<PenaltyPoint>,IPenaltyPointService
    {
        private readonly ApplicationDbContext _context;
        public PenaltyPointService(ApplicationDbContext context):base(context) 
        {
            _context = context; 
            
        }

     

        public  async Task<int> GetPointCountAsync(string userId)
        {
            var penaltyPointsrelatedToUser = _context.penaltyPoints.Where(x=>x.ApplicationUserId == userId).ToList();
            if (penaltyPointsrelatedToUser.Any())
            {
                var pentaltyPointsCount = penaltyPointsrelatedToUser.Sum(x => x.Points);
                return pentaltyPointsCount; 
            }
            return 0;


        }
    }
}
