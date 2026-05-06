
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DataServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapControllerRoute(
               name: "default",
               pattern: "{controller=api}/{action}");

            app.Run();
        }
    }
}
