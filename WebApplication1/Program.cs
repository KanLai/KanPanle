using System.Net;
using MessagePack;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using SignalRWebpack.Hubs;
using WebApplication1;
var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
const string hup = "/hub";
Console.ForegroundColor = ConsoleColor.DarkMagenta;
Console.WriteLine("服务正在启动...");
var options = new WebApplicationOptions
{
    Args = args,
    WebRootPath = $"{baseDirectory}/wwwroot",
};
Console.WriteLine("网站目录 -> " + options.WebRootPath);
var builder = WebApplication.CreateBuilder(options);


builder.Configuration.SetBasePath(baseDirectory).AddJsonFile("appsettings.json", false, true);
builder.Services.AddDataProtection()
    .SetApplicationName("8.kanlai.com.cn");
builder.Services.Configure<KestrelServerOptions>(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 8080); // 监听端口
    serverOptions.Limits.MaxRequestBodySize = 1024 * 1024 * 1000; // 1000MB
    Console.WriteLine("监听端口:8080");
});
builder.Services.AddDbContext<ApplicationDbContext>(contextOptionsBuilder =>
    contextOptionsBuilder.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AsyncAuthorizationFilter>()
    .AddScoped<AsyncModelValidationFilter>();
builder.Services.AddSingleton<ApiConfiguration>().AddSingleton<CachedConfigService>();
builder.Services.AddControllers(mvcOptions => { mvcOptions.Filters.AddService<AsyncModelValidationFilter>(); })
    .ConfigureApiBehaviorOptions(apiBehaviorOptions => { apiBehaviorOptions.SuppressModelStateInvalidFilter = true; });

builder.Services.AddMemoryCache();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
}

builder.Services.AddSignalR().AddMessagePackProtocol(protocolOptions =>
{
    protocolOptions.SerializerOptions = MessagePackSerializerOptions.Standard
        .WithResolver(new AesResolver())
        .WithSecurity(MessagePackSecurity.UntrustedData);
});
builder.Services.AddAuthentication(authenticationOptions =>
    {
        authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwtBearerOptions =>
    {
        jwtBearerOptions.TokenValidationParameters = new ApiConfiguration(builder.Configuration).GetTokenValidationParameters();
        jwtBearerOptions.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(hup))
                {
                    //解密token
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };

    });


var app = builder.Build();
Console.ForegroundColor = ConsoleColor.DarkGreen;
Console.WriteLine("服务启动中...");




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<ApiResponseMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHub<ChatHub>(hup, dispatcherOptions =>
{
    dispatcherOptions.Transports =
        HttpTransportType.WebSockets |
        HttpTransportType.LongPolling;
});
//app.UseHttpsRedirection();//关闭https

Console.WriteLine("服务启动成功");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();