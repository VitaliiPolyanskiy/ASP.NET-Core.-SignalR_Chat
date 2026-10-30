using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace SignalR_Chat;

/*
Ключовою сутністю в SignalR, через яку клієнти обмінюються повідомленнями 
із сервером та між собою, є хаб (hub). 
Хаб представляє певний клас, який успадкований від абстрактного класу 
Microsoft.AspNetCore.SignalR.Hub.
*/
public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> Users = new();

    // Відправлення повідомлень
    public async Task Send(string username, string message)
    {
        // Виклик методу AddMessage на всіх клієнтах
        await Clients.All.SendAsync("AddMessage", username, message);
    }

    // Підключення нового користувача
    public async Task Connect(string userName)
    {
        var id = Context.ConnectionId;

        if (Users.TryAdd(id, userName))
        {
            var allUsers = Users.Select(x => new User(x.Key, x.Value)).ToList();

            // Виклик методу Connected лише на поточному клієнті, який звернувся до сервера
            await Clients.Caller.SendAsync("Connected", id, userName, allUsers);

            // Виклик методу NewUserConnected на всіх клієнтах, окрім клієнта з певним id
            await Clients.AllExcept(id).SendAsync("NewUserConnected", id, userName);
        }
    }

    // OnDisconnectedAsync спрацьовує під час відключення клієнта.
    // Як параметр передається повідомлення про помилку, яке описує,
    // чому відбулося відключення.
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Users.TryRemove(Context.ConnectionId, out var userName))
        {
            // Виклик методу UserDisconnected на всіх клієнтах
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId, userName);
        }

        await base.OnDisconnectedAsync(exception);
    }
}