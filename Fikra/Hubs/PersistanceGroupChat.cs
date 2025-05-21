using Fikra.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SparkLink.Data;
using SparkLink.Models.Identity;
using SparkLink.Service.Implementation;
using SparkLink.Service.Interface;

namespace Fikra.Hubs
{
    public class PersistentGroupService :GenericRepo<ChatGroup>,IPresistanceGroupService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersistentGroupService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServices _identityService;
        public PersistentGroupService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IIdentityServices identityServices,
            ILogger<PersistentGroupService> logger):base(context)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _identityService = identityServices;
        }

        public async Task<ChatGroup> CreateGroupAsync(string ideaTitle, string ideaOwnerId)
        {
            try
            {
                var groupName = $"idea_{ideaTitle.Replace(" ", "_")}";

               
                var existingGroup = await _context.ChatGroups
                    .FirstOrDefaultAsync(g => g.Name == groupName);

                if (existingGroup != null)
                {
                    _logger.LogWarning($"Group {groupName} already exists");
                    return existingGroup;
                }

                var group = new ChatGroup
                {
                    Name = groupName,
                    IdeaTitle = ideaTitle,
                    IdeaOwnerId = ideaOwnerId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.ChatGroups.AddAsync(group);
                await _context.SaveChangesAsync();

      
                await AddMemberToGroupAsync(group.Id, ideaOwnerId);

                return group;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group");
                throw;
            }
        }

        public async Task AddMemberToGroupAsync(string groupId, string userId)
        {
            try
            {
             
                var existingMember = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.ApplicationUserId == userId);

                if (existingMember != null)
                {
                    _logger.LogWarning($"User {userId} is already in group {groupId}");
                    return;
                }
                var user=await _userManager.FindByIdAsync(userId);
                var groupMember = new GroupMember
                {
                    GroupId = groupId,
                    ApplicationUserId = userId,
                    JoinedAt = DateTime.UtcNow,
                    UserName=user.UserName,
                    
                    
                    
                };

                await _context.GroupMembers.AddAsync(groupMember);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding member to group");
                throw;
            }
        }

        public async Task RemoveMemberFromGroupAsync(string groupId, string userId)
        {
            try
            {
                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId== groupId && gm.ApplicationUserId == userId);

                if (member == null)
                {
                    _logger.LogWarning($"User {userId} not found in group {groupId}");
                    return;
                }

                _context.GroupMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing member from group");
                throw;
            }
        }

        public async Task<List<ChatGroup>> GetUserGroupsAsync(string userId)
        {
            try
            {
                return await _context.GroupMembers
                    .Where(gm => gm.ApplicationUserId == userId)
                    .Include(gm => gm.Group)
                    .Select(gm => gm.Group)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user groups");
                throw;
            }
        }

        public async Task<ChatGroup> GetGroupByIdAsync(string groupId)
        {
            try
            {
                return await _context.ChatGroups
                    .Include(g => g.Members)
                    .FirstOrDefaultAsync(g => g.Id == groupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting group by ID");
                throw;
            }
        }

        public async Task<bool> IsUserInGroup(string  groupId, string userId)
        {
            try
            {
                return await _context.GroupMembers
                    .AnyAsync(gm => gm.GroupId == groupId && gm.ApplicationUserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user is in group");
                throw;
            }
        }


        public async Task<List<GroupMember>> GetGroupMembersAsync(string groupId)
        {
            try
            {
                return await _context.GroupMembers
                    .Where(gm => gm.GroupId == groupId)
                    .Include(gm => gm.ApplicationUser) 
                    .Select(gm => new GroupMember
                    {
                        Id = gm.ApplicationUser.Id,
                        UserName = gm.ApplicationUser.UserName
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting group members");
                throw;
            }
        }

        public async Task<string> GetGroupIdAsync(string IdeaTitle)
        {
            var group= await _context.ChatGroups.FirstOrDefaultAsync(x => x.Name == IdeaTitle);
            return group.Id;
        }
        public async Task<List<ChatGroup>> GetGroupChatRelatedToUser(string userId)
        {
            var groups = await _context.ChatGroups.Include(x => x.Members).ToListAsync();
            var groupWithRelatedUser = groups.Where(g => g.Members.Any(m => m.ApplicationUserId == userId)).ToList();
            return groupWithRelatedUser;

        }
    }
}
