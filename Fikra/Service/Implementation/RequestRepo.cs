using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class RequestRepo:GenericRepo<Request>,IRequestRepo

    {
        private readonly ApplicationDbContext _context;
       public  RequestRepo(ApplicationDbContext context):base(context) 
            {

            }
    }
}
