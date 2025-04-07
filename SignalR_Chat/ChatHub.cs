using Microsoft.AspNetCore.SignalR;

namespace SignalR_Chat
{
    /*
    Ключевой сущностью в SignalR, через которую клиенты обмениваются сообщеними 
    с сервером и между собой, является хаб (hub). 
    Хаб представляет некоторый класс, который унаследован от абстрактного класса 
    Microsoft.AspNetCore.SignalR.Hub.
    */
    public class ChatHub : Hub
    {
        static List<User> Users = new List<User>();

        // Отправка сообщений
        public async Task Send(string username, string message)
        {
            // Вызов метода AddMessage на всех клиентах
            await Clients.All.SendAsync("AddMessage", username, message);
        }

        // Подключение нового пользователя
        public async Task Connect(string userName)
        {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.ConnectionId == id))
            {
                Users.Add(new User { ConnectionId = id, Name = userName });

                // Вызов метода Connected только на текущем клиенте, который обратился к серверу
                await Clients.Caller.SendAsync("Connected", id, userName, Users);

                // Вызов метода NewUserConnected на всех клиентах, кроме клиента с определенным id
                await Clients.AllExcept(id).SendAsync("NewUserConnected", id, userName);
            }
        }

        // OnDisconnectedAsync срабатывает при отключении клиента.
        // В качестве параметра передается сообщение об ошибке, которая описывает,
        // почему произошло отключение.
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                Users.Remove(item);
                var id = Context.ConnectionId;
                // Вызов метода UserDisconnected на всех клиентах
                await Clients.All.SendAsync("UserDisconnected", id, item.Name);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
