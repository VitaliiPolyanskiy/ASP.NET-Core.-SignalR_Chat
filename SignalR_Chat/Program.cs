using SignalR_Chat;   // простір імен класу ChatHub

var builder = WebApplication.CreateBuilder(args);

// Для використання функціональності бібліотеки SignalR,
// у програмі необхідно зареєструвати відповідні сервіси
builder.Services.AddSignalR();

var app = builder.Build();

// За допомогою спеціального методу розширення UseDefaultFiles() можна налаштувати
// відправлення статичних веб-сторінок за замовчуванням без звернення до них за повним шляхом.
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chat");   // ChatHub буде обробляти запити за шляхом /chat

app.Run();