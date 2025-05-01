using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class IdeaRatingService: GenericRepo<IdeaRating>,IIdeaRating
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public IdeaRatingService(ApplicationDbContext applicationDbContext):base(applicationDbContext) 
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
