using Microsoft.AspNetCore.SignalR;

namespace SignalRServer_Core
{
    public class ContosoChatHub : Hub
    {
        public async Task SendMessageToAll(string message)
        {
            await Clients.Others.SendAsync("GetMessage", message);
        }
    }
}
