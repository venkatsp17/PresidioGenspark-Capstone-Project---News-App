using Microsoft.AspNetCore.SignalR;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace NewsApp.Services.Classes
{
    public class CommentHub : Hub
    {
        [ExcludeFromCodeCoverage]
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }
        [ExcludeFromCodeCoverage]
        public async Task JoinGroup(string articleId)
        {
            try
            {
                if (Regex.IsMatch(articleId, @"^[a-zA-Z0-9\-]+$"))
                {
                    Console.WriteLine($"Joining group: {articleId}");
                    await Groups.AddToGroupAsync(Context.ConnectionId, articleId);
                    await Clients.Caller.SendAsync("GroupJoined", articleId);
                }
                else
                {
                    Console.WriteLine($"Invalid group name: {articleId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error joining group: {ex.Message}");
            }
        }
        [ExcludeFromCodeCoverage]
        public async Task LeaveGroup(string articleId)
        {
            try
            {
                Console.WriteLine($"Leaving group: {articleId}");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, articleId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leaving group: {ex.Message}");
            }
        }
        [ExcludeFromCodeCoverage]
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
