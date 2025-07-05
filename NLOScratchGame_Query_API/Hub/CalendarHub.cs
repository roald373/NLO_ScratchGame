using Microsoft.AspNetCore.SignalR;
using NLO_ScratchGame_Contracts;

namespace NLOScratchGame_Query_API.Hub
{
    public class CalendarHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Client connected: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task CellScratched(ScratchResult result)
        {
            await Clients.All.SendAsync("CellScratched", result);
        }
    }
}
