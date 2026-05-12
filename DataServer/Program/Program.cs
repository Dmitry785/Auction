
using Application.Interfaces;
using Application.Services;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("SqliteConnectionString");
            builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite(connectionString));
            builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            builder.Services.AddTransient<CommonService>(); 
            builder.Services.AddScoped<CmdService>();
            builder.Services.AddHostedService<CmdListenerService>();

            var app = builder.Build();

            app.UseStaticFiles();

            app.Use(async (c, n) =>
            {
                c.Request.EnableBuffering();
                using (StreamReader sr = new StreamReader(
                    c.Request.Body,
                    encoding: System.Text.Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true))
                {
                    var body = await sr.ReadToEndAsync();
                    Console.WriteLine($"{c.Connection.RemoteIpAddress?.MapToIPv4()} " +
                        $">> {c.Request.Path}\nbody: {(body.Length == 0 ? "no body" : body)}");
                    c.Request.Body.Position = 0;
                }
                await n.Invoke();
            });

            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.MapControllers();

            app.Run();
        }
    }
}
