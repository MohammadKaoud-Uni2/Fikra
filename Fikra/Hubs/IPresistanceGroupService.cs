using Fikra.Models;
using SparkLink.Service.Interface;

namespace Fikra.Hubs
{
    public interface IPresistanceGroupService:IGenericRepo<ChatGroup>
    {
        Task<ChatGroup> CreateGroupAsync(string ideaTitle, string ideaOwnerId);
        Task AddMemberToGroupAsync(string groupId, string userId);
        Task RemoveMemberFromGroupAsync(string groupId, string userId);
        Task<List<ChatGroup>> GetUserGroupsAsync(string userId);
        Task<ChatGroup> GetGroupByIdAsync(string groupId);
        Task<bool> IsUserInGroup(string groupId, string userId);
        Task<List<GroupMember>> GetGroupMembersAsync(string groupId);
        public Task<string>GetGroupIdAsync(string IdeaTitle);
        public Task<List<ChatGroup>> GetGroupChatRelatedToUser(string UserId);

    }
}
