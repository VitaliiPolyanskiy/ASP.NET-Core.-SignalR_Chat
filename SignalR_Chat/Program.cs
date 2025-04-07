using SignalR_Chat;   // пространство имен класса ChatHub

var builder = WebApplication.CreateBuilder(args);
// ƒл€ использовани€ функциональности библиотеки SignalR,
// в приложении необходимо зарегистрировать соответствующие сервисы
builder.Services.AddSignalR();  

var app = builder.Build();
// — помощью специального метода расширени€ UseDefaultFiles() можно настроить
// отправку статических веб-страниц по умолчанию без обращени€ к ним по полному пути.
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chat");   // ChatHub будет обрабатывать запросы по пути /chat

app.Run();
