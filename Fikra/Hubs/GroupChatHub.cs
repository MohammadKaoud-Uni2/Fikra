using Fikra.Models;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using System.Text.RegularExpressions;

namespace Fikra.Hubs
{
    public class GroupChatHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        private readonly IPresistanceGroupService _groupService;
        private readonly IMessageRepo _messageService;
        private readonly IIdentityServices _identityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRSAService _raService;
        private readonly IGroupMessageService _groupMessageService;
        public GroupChatHub(
            PresenceTracker presenceTracker,
            IPresistanceGroupService groupService,
            IMessageRepo messageService,
            UserManager<ApplicationUser> userManager,
            IRSAService rsAService,
            IGroupMessageService groupMessageService,
            IIdentityServices IdentityService)
        {
            _userManager = userManager;
            _presenceTracker = presenceTracker;
            _groupService = groupService;
            _messageService = messageService;
            _identityService = IdentityService;
            _raService = rsAService;
            _groupMessageService = groupMessageService;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User.Identity.Name;
            var connectionId = Context.ConnectionId;

         
            await _presenceTracker.UserConnected(username, connectionId);

            await RejoinPersistentGroups(username, connectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User.Identity.Name;
            var connectionId = Context.ConnectionId;

            // Remove from presence tracker
            await _presenceTracker.UserDisconnected(username, connectionId);

            await base.OnDisconnectedAsync(exception);
        }

        private async Task RejoinPersistentGroups(string username, string connectionId)
        {
            var user = await _userManager.FindByNameAsync(username);
            var groups = await _groupService.GetUserGroupsAsync(user.Id);

            foreach (var group in groups)
            {
                await Groups.AddToGroupAsync(connectionId, group.Name);
                await Clients.Caller.SendAsync("GroupRejoined", group.Id, group.Name);
            }
        }
        public async Task JoinGroup(string groupId)
        {
            var username = Context.User.Identity.Name;
            var group = await _groupService.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                await Clients.Caller.SendAsync("Error", "Group not found");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, group.Name);
            await Clients.Caller.SendAsync("UserJoined", username);

            // Only get messages for this specific group
            var messages = await _groupMessageService.GetGroupMessageByGroupId(groupId);
            if (messages != null)
            {
                foreach (var msg in messages.OrderBy(m => m.SentAt))
                {
                    var decrypted = _raService.DecryptData(msg.message);
                    var formatted = $"{msg.SenderName}:{decrypted}";
                    await Clients.Caller.SendAsync("ReceiveGroupMessage", formatted);
                }
            }
        }



        public async Task LeaveGroup(string groupId)
        {
            var username = Context.User.Identity.Name;
            var group = await _groupService.GetGroupByIdAsync(groupId);

            if (group == null)
            {
                await Clients.Caller.SendAsync("Error", "Group not found");
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Name);
            await Clients.Caller.SendAsync("LeftGroup", groupId);

            // Notify other group members
            await Clients.OthersInGroup(group.Name).SendAsync("UserLeft", username);
        }

        public async Task SendGroupMessage(string groupId, string message)
        {
            var username = Context.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                await Clients.Caller.SendAsync("Error", "User not found");
                return;
            }

            var group = await _groupService.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                await Clients.Caller.SendAsync("Error", "Group not found");
                return;
            }

          
            var isMember = await _groupService.IsUserInGroup(groupId, user.Id);
            if (!isMember)
            {
                await Clients.Caller.SendAsync("Error", "You are not a member of this group");
                return;
            }
            var editmessage = $"{username}:{message}";
            
            await Clients.Group(group.Name).SendAsync("ReceiveGroupMessage",editmessage);
           
            var groupMessage = new GroupMessage
            {
                ChatGroup = group,
                ChatGroupId = groupId,
                message = _raService.EncryptData(message),
                SenderName = username,
                SentAt = DateTime.Now,

            };
       await   _groupMessageService.AddAsync(groupMessage);
        await _groupMessageService.SaveChangesAsync();
            group.Messages.Add(groupMessage);
          await   _groupService.UpdateAsync(group);
            await _groupService.SaveChangesAsync();
            
        }

        public async Task AddUserToGroup(string groupId, string userId)
        {
            var currentUser = Context.User.Identity.Name;
            var group = await _groupService.GetGroupByIdAsync(groupId);

         
            if (group.IdeaOwnerId != currentUser)
            {
                await Clients.Caller.SendAsync("Error", "Only group owner can add members");
                return;
            }

         
            await _groupService.AddMemberToGroupAsync(groupId, userId);

        
            var userConnections = await _presenceTracker.GetConnectionIdsForUser(userId);
            if (userConnections != null)
            {
                
                foreach (var connectionId in userConnections)
                {
                    await Groups.AddToGroupAsync(connectionId, group.Name);
                }
            }

        
            var user = await _userManager.FindByNameAsync(userId);
            await Clients.Group(group.Name).SendAsync("UserAdded", new
            {
                GroupId = group.Id,
                UserId = user.Id,
                UserName = user.UserName
            });
        }

        public async Task RemoveUserFromGroup(string groupId, string userId)
        {
            var currentUser = Context.User.Identity.Name;
            var group = await _groupService.GetGroupByIdAsync(groupId);

       
            if (group.IdeaOwnerId != currentUser)
            {
                await Clients.Caller.SendAsync("Error", "Only group owner can remove members");
                return;
            }

         
            await _groupService.RemoveMemberFromGroupAsync(groupId, userId);

            var userConnections = await _presenceTracker.GetConnectionIdsForUser(userId);
            if (userConnections != null)
            {
             
                foreach (var connectionId in userConnections)
                {
                    await Groups.RemoveFromGroupAsync(connectionId, group.Name);
                }
            }

      
            await Clients.Group(group.Name).SendAsync("UserRemoved", new
            {
                GroupId = group.Id,
                UserId = userId
            });

          
            await Clients.User(userId).SendAsync("RemovedFromGroup", group.Id);
        }
    }
    public class sendingMessageDto
    {
        public string Sender { get; set; }
        public string Content { get; set; }
    }
}
