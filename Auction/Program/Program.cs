using Application;
using Infrastructure;
using MediatR;
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

            builder.Services.AddTransient<DefaultDataHelper>();

            var app = builder.Build();

            app.UseStaticFiles();

            app.MapControllers();

            app.Services.GetRequiredService<DefaultDataHelper>().AddDefaultData();

            app.Run();
        }
    }
}
public class DefaultDataHelper
{
    private readonly IMediator _mediator;
    public DefaultDataHelper(IMediator mediator)
    {
        _mediator = mediator;
    }
    public void AddDefaultData()
    {

    }
}
