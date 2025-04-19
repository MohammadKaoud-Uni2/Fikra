using Fikra.Models;
using Fikra.Service.Interface;
using Microsoft.EntityFrameworkCore;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class MessageRepo:GenericRepo<Message>,IMessageRepo
        
    {
       private readonly ApplicationDbContext _context;
        private readonly IRSAService _IrsService;
        public MessageRepo(ApplicationDbContext context, IRSAService irsService) : base(context)
        {
            _context = context;
            _IrsService = irsService;
        }

        public async  Task<List<Message>> GetUndeliveredMessages(string recipientUsername)
        {
            var messages = await  _context.Messages
    .AsNoTracking()
    .Where(x => x.ReciverUserName == recipientUsername &&x.IsRead==false)
    .ToListAsync();

            if (messages.Any())
            {

                foreach (var msg in messages)
                {
                    msg.Content = _IrsService.DecryptData(msg.Content);
                    msg.IsRead = true; 
                }
                _context.Messages.UpdateRange(messages);
                await _context.SaveChangesAsync();

                return messages;
            }
            
            return null;


        }

        public async  Task UpdateStatueofMessages(List<Message> messages)
        {
            foreach (var msg in messages)
            {
                msg.IsRead = true;  
            }
            _context.Update(messages);   
            await _context.SaveChangesAsync();
        }
    }
}
