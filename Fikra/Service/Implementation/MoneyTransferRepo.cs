using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class MoneyTransferRepo:GenericRepo<MoneyTransferRequest>,IMoneyTransferRepo
    {
        private readonly ApplicationDbContext _context;
        public MoneyTransferRepo(ApplicationDbContext context) : base(context)
        {

        }
    }
}
