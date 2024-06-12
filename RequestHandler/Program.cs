using RequestHandler;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class Program
{
    public static async Task Main(string[] args)
    {
        var isService = !(Debugger.IsAttached || args.Contains("--console"));

        var builder = CreateHostBuilder(args.Where(arg => arg != "--console").ToArray());

        if (isService)
        {
            await builder.UseWindowsService().Build().RunAsync();
        }
        else
        {
            var host = builder.Build();
            await host.RunAsync();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                          .UseUrls("http://localhost:5000"); // Configure to listen on localhost
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
}