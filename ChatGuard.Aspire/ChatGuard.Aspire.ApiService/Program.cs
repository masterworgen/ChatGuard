using ChatGuard.Infrastructure;
using ChatGuard.Infrastructure.Background;
using ChatGuard.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOptions<UserConfiguration>().BindConfiguration(UserConfiguration.Path);

builder.Services.AddProblemDetails();

builder.Services.AddScoped<IUpdateHandler, ChatTrackerService>();
builder.Services.AddScoped<BotService>();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("sqlite")));

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var config = sp.GetService<IOptions<UserConfiguration>>()?.Value!;
    var dbConnection = new Microsoft.Data.Sqlite.SqliteConnection(builder.Configuration.GetConnectionString("sqliteBot"));
    return new WTelegramBotClient(config.BotToken, config.ApiId, config.ApiHash, dbConnection);
});

builder.Services.AddHostedService<BotBackground>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// ѕрименение миграций при запуске приложени€
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    dbContext.Database.Migrate();
}

app.UseExceptionHandler();

app.MapGet("/dismiss-user", async (string username, BotService service) =>
{
    var result = await service.RemoveUserFromChatsAsync($"{username}", CancellationToken.None);
    return result ? Results.Ok() : Results.Problem();
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapDefaultEndpoints();

app.Run();