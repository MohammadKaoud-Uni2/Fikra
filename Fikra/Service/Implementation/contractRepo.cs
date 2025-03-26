using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class contractRepo:GenericRepo<Contract>,IContractRepo
        
    {
        public contractRepo(ApplicationDbContext applicationDbContext):base(applicationDbContext) { 

        }
    }
}
