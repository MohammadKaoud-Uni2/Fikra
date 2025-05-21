using Fikra.Models;
using SparkLink.Service.Interface;

namespace Fikra.Service.Interface
{
    public interface IPenaltyPointService:IGenericRepo<PenaltyPoint>
    {

      public   Task<int> GetPointCountAsync(string userId);

    }
}
