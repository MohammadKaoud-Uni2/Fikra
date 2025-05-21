using System.Collections.Concurrent;

namespace Fikra.Hubs
{
    public class PresenceTracker
    {
        private readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();
        private readonly object _syncLock = new object();

        public Task UserConnected(string userName, string userConnectionId)
        {
            lock (_syncLock)
            {
                if (OnlineUsers.TryGetValue(userName, out var connections))
                {
                    if (!connections.Contains(userConnectionId))
                    {
                        connections.Add(userConnectionId);
                    }
                }
                else
                {
                    OnlineUsers.Add(userName, new List<string> { userConnectionId });
                }
            }
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string userName, string userConnectionId)
        {
            lock (_syncLock)
            {
                if (!OnlineUsers.TryGetValue(userName, out var connections))
                    return Task.CompletedTask;

                connections.Remove(userConnectionId);

                if (connections.Count == 0)
                {
                    OnlineUsers.Remove(userName);
                }
            }
            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            lock (_syncLock)
            {
                return Task.FromResult(OnlineUsers
                    .OrderBy(k => k.Key)
                    .Select(k => k.Key)
                    .ToArray());
            }
        }

        // New method to get all connection IDs for a specific user
        public Task<List<string>> GetConnectionIdsForUser(string userName)
        {
            lock (_syncLock)
            {
                if (OnlineUsers.TryGetValue(userName, out var connections))
                {
                    return Task.FromResult(new List<string>(connections)); // Return a copy
                }
                return Task.FromResult(new List<string>());
            }
        }

        // Optional: Method to get all active connection IDs (across all users)
        public Task<List<string>> GetAllConnectionIds()
        {
            lock (_syncLock)
            {
                return Task.FromResult(OnlineUsers
                    .SelectMany(kvp => kvp.Value)
                    .ToList());
            }
        }
    }
}
