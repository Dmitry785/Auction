using Application;
using Infrastructure;
namespace Auction
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddInfastructureLayer(builder.Configuration);

            builder.Services.AddApplicationLayer();

            var app = builder.Build();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
