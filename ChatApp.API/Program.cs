using ChatApp.API.Helpers;
using ChatApp.API.Hubs;
using ChatApp.API.Middlewares;
using ChatApp.Infrastructure.DataAccess;
using ChatApp.Infrastructure.DataAccess.Entities;
using ChatApp.Infrastructure.Services;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.OptionsPattern();
builder.DependencyInjection();

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//Cache na endpoint
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("Expire20", builder =>
        builder.Expire(TimeSpan.FromSeconds(15)));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
});

//Authentication Authorization
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.AddAuthorizationBuilder();

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ChatDbContext>()
    .AddApiEndpoints();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("User", policy => policy.RequireRole("User", "Administrator"));
});

//SignalR
builder.Services.AddSignalR(options =>
{
    
});

//Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddHangfireServer();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.AddRateLimiterHelper();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Migracja i role
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    var serviceProvider = scope.ServiceProvider.GetRequiredService<IServiceProvider>();
    dbContext.Database.Migrate();
    await DbSeeder.Seed(serviceProvider);

    app.UseHangfireDashboard();
    app.UseHangfireServer();
    RecurringJob.AddOrUpdate(
        "Wyczyść wiadomości i pokoje",
        () => scope.ServiceProvider.GetRequiredService<HangfireService>().CleaningMessagesAndCustomRooms(),
        "0 3 * * *",
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
}

app.UseHttpsRedirection();

app.UseCors("default");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRateLimiter();
app.UseMiddleware<UserAgentMiddleware>();

app.MapGroup("/identity").MapIdentityApi<User>();
app.MapHub<ChatHub>("/chathub");

app.MapControllers();

app.Run();
