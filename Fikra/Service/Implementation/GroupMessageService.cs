using Fikra.Models;
using Fikra.Service.Interface;
using Microsoft.EntityFrameworkCore;
using SparkLink.Data;
using SparkLink.Service.Implementation;

namespace Fikra.Service.Implementation
{
    public class GroupMessageService:GenericRepo<GroupMessage>,IGroupMessageService
        
    {
        private readonly ApplicationDbContext _context;
        public GroupMessageService(ApplicationDbContext context) : base(context) { 
            _context = context;
        }

        public async Task<List<GroupMessage>> GetGroupMessageByGroupId(string groupId)
        {
            var messages=await _context.GroupesMessages.Where(x=>x.ChatGroupId==groupId).ToListAsync();
            if (messages.Any())
            {
                return messages;
            }
            return null;
        }
    }

}
