using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop
{
    /// <summary>
    /// SignalR Hub — real-time inventory updates.
    ///
    /// Groups:
    ///   "admin"  — admin/manager browsers receive InventoryUpdated events.
    ///   "public" — public clients (optional, for product pages).
    ///
    /// Clients subscribe on connect (JS): connection.invoke("JoinGroup", "admin").
    /// </summary>
    public class InventoryHub : Hub
    {
        private readonly ILogger<InventoryHub> _logger;

        public InventoryHub(ILogger<InventoryHub> logger)
        {
            _logger = logger;
        }

        /// <summary>Client join a group to receive targeted events.</summary>
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogDebug("[SignalR] {ConnectionId} joined group '{Group}'",
                Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogDebug("[SignalR] Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogDebug("[SignalR] Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
