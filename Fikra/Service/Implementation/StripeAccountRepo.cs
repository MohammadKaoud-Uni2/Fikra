using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class StripeAccountRepo:GenericRepo<StripeAccount>,IStripeAccountsRepo
    {

        public StripeAccountRepo(ApplicationDbContext context) : base(context)
        {

        }
    }
}
