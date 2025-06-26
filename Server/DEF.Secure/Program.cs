using System.Net;

namespace DEF;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 5005, listenOptions => { });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}