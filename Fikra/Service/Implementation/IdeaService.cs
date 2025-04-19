using Fikra.Models;
using Fikra.Service.Interface;
using OpenAI;
using SparkLink.Data;
using SparkLink.Service.Implementation;
namespace Fikra.Service.Implementation
{
    public class IdeaService:GenericRepo<Idea>,IIdeaService
    {
       
        private readonly ApplicationDbContext _context;
        public IdeaService(ApplicationDbContext context):base(context)
        {
            _context = context;

        }


    }
}
