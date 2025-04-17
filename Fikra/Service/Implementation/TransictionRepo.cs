using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class TransactionRepo:GenericRepo<Transaction>,ITransictionRepo
    {
        private readonly ApplicationDbContext _context;
        public TransactionRepo(ApplicationDbContext context) : base(context)
        {

        }
    }
}
