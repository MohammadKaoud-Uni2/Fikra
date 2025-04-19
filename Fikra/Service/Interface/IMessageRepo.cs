using Fikra.Models;
using SparkLink.Service.Interface;

namespace Fikra.Service.Interface
{
    public interface IMessageRepo:IGenericRepo<Message>
    {
        Task<List<Message>> GetUndeliveredMessages(string recipientUsername);
        public Task UpdateStatueofMessages(List<Message> messages);
    }
}
