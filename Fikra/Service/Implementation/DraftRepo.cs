using Fikra.Controllers;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class DraftRepo:GenericRepo<Draft>, IDraftRepo
    {
        private readonly ApplicationDbContext _context;
        public DraftRepo(ApplicationDbContext context) : base(context) {
            _context = context;
        }
    }
}
