
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DataServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "Authentication";
                });

            builder.Services.AddAuthorization(p =>
            {

            });

            var app = builder.Build();

            app.UseAuthorization();
            app.UseAuthentication();

            app.Run();
        }
    }
}
