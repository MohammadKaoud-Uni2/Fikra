using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Fikra.Service.Interface;
using Fikra.Models;
using Microsoft.AspNetCore.Identity;
using SparkLink.Models.Identity;
using Fikra.Models.Dto;

namespace Fikra.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ContractHub : Hub
    {
        private static ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>(); 
       private readonly IRequestRepo _requestRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageRepo _messageRepo;
        private readonly IRSAService _IrsService;
        private readonly IJoinRequestService _joinRequestService;
        private readonly PresenceTracker _presenceTracker;
        public ContractHub(IRequestRepo requestRepo,UserManager<ApplicationUser> userManager, IMessageRepo messageRepo, IRSAService irsService,IJoinRequestService joinRequestService,PresenceTracker presenceTracker)
        {
            _requestRepo = requestRepo;
            _userManager = userManager;
            _messageRepo = messageRepo;
            _IrsService = irsService;
            _joinRequestService = joinRequestService;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
       

            if (!string.IsNullOrEmpty(username))
            {
                Users[username] = Context.ConnectionId;
                await _presenceTracker.UserConnected(username, Context.ConnectionId);

                await Clients.Caller.SendAsync("ReceiveMessage", $"Welcome {username}! You are now connected.");

               
                var undelivered=await _messageRepo.GetUndeliveredMessages(username);
                if (undelivered != null)
                {
                 
                    foreach (var message in undelivered)
                    {
                        await Clients.Caller.SendAsync("ReceiveMessage", $"{message.SenderUserName}:  {message.Content}");

                    }

                }
               


            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Authentication failed. Please log in.");
                Context.Abort();
            }
        }
      
       
        public async Task SendMessageToUser(string recipientUsername, string message)
        {
            var senderUsername = Context.User?.Identity?.Name;

            if (senderUsername.Equals(recipientUsername, StringComparison.OrdinalIgnoreCase))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "You cannot send a message to yourself.");
                return;
            }

            if (string.IsNullOrEmpty(senderUsername))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "You need to log in first.");
                return;
            }

            var newMessage = new Message
            {
                Id = Guid.NewGuid().ToString(),
                SenderUserName = senderUsername,
                ReciverUserName = recipientUsername,
                Content = _IrsService.EncryptData(message),
                SendAt = DateTime.UtcNow,
                IsRead=false,
               
            };

            if (Users.TryGetValue(recipientUsername, out var recipientConnectionId))
            {
                await Clients.Client(recipientConnectionId).SendAsync("ReceiveMessage", $"{senderUsername}: {message}");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "User Is Not here Message Was Encrypted in Db.");
                await _messageRepo.AddAsync(newMessage);
                await _messageRepo.SaveChangesAsync();
            }
        }
        [Authorize(Roles = "Investor")]
        public async Task SendInvestmentRequest(string recipientUsername, string requestDetails)
        {
            var senderUsername = Context.User?.Identity?.Name;

            if (senderUsername.Equals(recipientUsername, StringComparison.OrdinalIgnoreCase))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "You cannot send a request to yourself.");
                return;
            }

            if (string.IsNullOrEmpty(senderUsername))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "You need to log in first.");
                return;
            }
            var IdeaOwnerCheck=await _userManager.FindByNameAsync(recipientUsername);
            if (IdeaOwnerCheck == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "This IdeaOwner is Not found");
                return;

            }
            var IdeaOwnerCheckRole=await _userManager.GetRolesAsync(IdeaOwnerCheck);
            if (!IdeaOwnerCheckRole.Contains("IdeaOwner"))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "This Person is Not Consider as IdeaOwner");
                return;
            }
            var SenderCheck=await _userManager.FindByNameAsync(senderUsername);

            var investmentRequest = new Request
            {
                Investor = SenderCheck,
                InvestorId = SenderCheck.Id,
                IdeaOwner = IdeaOwnerCheck,
                IdeaOwnerId = IdeaOwnerCheck.Id,
                Status = "Pending",
                InvestorName = senderUsername,
                IdeaOwnerName = recipientUsername,
                RequestDetail = _IrsService.EncryptData(requestDetails)
                




            };

            if (Users.TryGetValue(recipientUsername, out var recipientConnectionId))
            {
              
                await Clients.Client(recipientConnectionId).SendAsync("ReceiveInvestmentRequest",
                    senderUsername,
                    requestDetails);
            }
            else
            {
        
                await _requestRepo.AddAsync(investmentRequest);
                await _requestRepo.SaveChangesAsync();
                await Clients.Caller.SendAsync("ReceiveMessage",
                    "Request sent successfully. Recipient is currently offline and will see it when they come online.");
            }
        }

        [Authorize(Roles ="IdeaOwner")]
        public async Task AcceptInvestmentRequest(string senderUsername)
        {
            var recipientUsername = Context.User?.Identity?.Name;
            if (senderUsername.Equals(recipientUsername, StringComparison.OrdinalIgnoreCase))
            {
                await Clients.Caller.SendAsync("ReciveMessage", "You Cannot Accept Request from yourself");
                return;
            }

            if (string.IsNullOrEmpty(recipientUsername))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "You need to log in first.");
                

                return;
            }

            if (Users.TryGetValue(senderUsername, out var senderConnectionId))
            {


               var request=await  _requestRepo.GetRequestBetweenTwoUsers(recipientUsername, senderUsername);
                await _requestRepo.UpdateRequest(request);
               
                await Clients.Client(senderConnectionId).SendAsync("InvestmentRequestAccepted", recipientUsername);
            }
        }
        [Authorize(Roles ="IdeaOwner")]
        public async Task RejectInvestmentRequest(string senderUsername)
        {
            var recipientUsername = Context.User?.Identity?.Name;
            if (senderUsername.Equals(recipientUsername, StringComparison.OrdinalIgnoreCase))
            {
                await Clients.Caller.SendAsync("ReciveMessage", "You Cannot Accept Request from yourself");
                return;

            }
          

            if (string.IsNullOrEmpty(recipientUsername))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "You need to log in first.");
                return;
            }

            if (Users.TryGetValue(senderUsername, out var senderConnectionId))
            {
                var request = await _requestRepo.GetRequestBetweenTwoUsers(recipientUsername, senderUsername);
                await _requestRepo.RejectRequest(request);
                await Clients.Client(senderConnectionId).SendAsync("InvestmentRequestRejected", recipientUsername);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = Users.FirstOrDefault(u => u.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(username))
            {
                await _presenceTracker.UserDisconnected(username, Context.ConnectionId);
                Users.TryRemove(username, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }
      
        public async Task ReceiveJobRequest(string message)
        {
           
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }
    }
}
