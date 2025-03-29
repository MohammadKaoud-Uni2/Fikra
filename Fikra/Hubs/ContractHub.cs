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

namespace Fikra.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ContractHub : Hub
    {
        private static ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>(); 
       private readonly IRequestRepo _requestRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public ContractHub(IRequestRepo requestRepo,UserManager<ApplicationUser> userManager)
        {
            _requestRepo=requestRepo;
            _userManager=userManager;

        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                Users[username] = Context.ConnectionId;
                await Clients.Caller.SendAsync("ReceiveMessage", $"Welcome {username}! You are now connected.");
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
                await Clients.Caller.SendAsync("ReciveMessage", "You Cannot Accept Request from yourself");
                return;

            }

            if (string.IsNullOrEmpty(senderUsername))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "You need to log in first.");
                return;
            }

            if (Users.TryGetValue(recipientUsername, out var recipientConnectionId))
            {
                await Clients.Client(recipientConnectionId).SendAsync("ReceiveMessage", $"{senderUsername}: {message}");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "User is offline or not found.");
            }
        }
        [Authorize(Roles = "Investor")]
        public async Task SendInvestmentRequest(string recipientUsername, string requestDetails)
        {
            var senderUsername = Context.User?.Identity?.Name;
            if (senderUsername.Equals(recipientUsername, StringComparison.OrdinalIgnoreCase))
            {
                await Clients.Caller.SendAsync("ReciveMessage", "You Cannot Accept Request from yourself");
                return;

            }


            if (string.IsNullOrEmpty(senderUsername))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "You need to log in first.");
                return;
            }

            if (Users.TryGetValue(recipientUsername, out var recipientConnectionId))
            {
                await Clients.Client(recipientConnectionId).SendAsync("ReceiveInvestmentRequest", senderUsername, requestDetails);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "User is offline or not found.");
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
                var firstUser = await _userManager.FindByNameAsync(senderUsername);
                var secondUser = await _userManager.FindByNameAsync(recipientUsername);

                var newRequest = new Request()
                {
                    IdeaOwner=secondUser,
                    IdeaOwnerId=secondUser.Id,
                    Investor=firstUser,
                    InvestorId=firstUser.Id,
                };
                await _requestRepo.AddAsync(newRequest);
                await _requestRepo.SaveChangesAsync();
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
                await Clients.Client(senderConnectionId).SendAsync("InvestmentRequestRejected", recipientUsername);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = Users.FirstOrDefault(u => u.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(username))
            {
                Users.TryRemove(username, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
