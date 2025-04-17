using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;
using SparkLink.Service.Interface;

namespace Fikra.Service.Implementation
{
    public class StripeCustomerRepo:GenericRepo<StripeCustomer>,IStripeCustomer
    {
        private readonly ApplicationDbContext _context;
        public StripeCustomerRepo(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
