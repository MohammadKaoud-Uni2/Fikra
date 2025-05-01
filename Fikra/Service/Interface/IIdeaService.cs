using Fikra.Models;
using SparkLink.Service.Interface;

namespace Fikra.Service.Interface
{
    public interface IIdeaService:IGenericRepo<Idea>
    {
       
        public Task<FullProjectAnalysis> AnalyzeIdeaCompletely(Idea idea);
        public Task<double> TestNvp(List<CashFlow> flows, double discountRate);
        public Task<List<Idea>> GetIdeasWithRating();
        public Task<List<Idea>> GetIdeasRelatedToSpecificIdeaOwner(string IdeaOwnerName);
        public Task<double> TestIRR(List<CashFlow> flows, double minRate = -0.5, double maxRate = 1.0, double tolerance = 0.0001, int maxIterations = 1000);


    }
}
