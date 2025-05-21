using Fikra.Models;
using SparkLink.Service.Interface;

namespace Fikra.Service.Interface
{
    public interface IGroupMessageService:IGenericRepo<GroupMessage>
    {
        public Task<List<GroupMessage>>GetGroupMessageByGroupId(string groupId);
        
    }
}
