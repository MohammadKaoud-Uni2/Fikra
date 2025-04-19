
using Fikra.Models;
using SparkLink.Service.Interface;

namespace Fikra.Service.Interface
{
    public interface IRequestRepo:IGenericRepo<Request>
    {
        public Task<List<Request>> GetRequestByUserName(string UserName);
        public Task<Request>GetRequestBetweenTwoUsers(string IdeaOwnerUserName, string InvestorUserName);
        public Task UpdateRequest(Request request);
        public Task RejectRequest(Request request);
    }
}
