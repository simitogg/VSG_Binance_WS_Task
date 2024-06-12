using DataCollector;
using Commons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ServiceProcess;

public class Program
{
    static async Task Main(string[] args)
    {

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddDbContext<SharedContext>(options =>
                //options.UseNpgsql("Host=your_host;Database=your_db;Username=your_user;Password=your_password"))
                options.UseMySQL(configuration.GetConnectionString("DefaultConnection")))
            .BuildServiceProvider();

        if (!Environment.UserInteractive)
        {
            ServiceBase[] ServicesToRun;
            var dbContext = serviceProvider.GetRequiredService<SharedContext>();
            ServicesToRun = new ServiceBase[]
            {
                new Service(dbContext)
            };
            ServiceBase.Run(ServicesToRun);
        }
        else
        {
            var dbContext = serviceProvider.GetRequiredService<SharedContext>();
            Start(args, dbContext);

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey(true);

            Stop(dbContext);
        }
    }

    public static async void Start(string[] args, SharedContext context)
    {
        // Start the writing process
        var binanceService = new WSService(context);
        binanceService.StartAsync().GetAwaiter().GetResult();
    }

    public static void Stop(SharedContext context)
    {
        // Stop the writing process
        context.Dispose();
    }
}