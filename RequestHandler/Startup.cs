using Commons;
using Microsoft.EntityFrameworkCore;
using RequestHandler.BusinessLayer;
using System.Text.Json;

namespace RequestHandler
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true; // Respect the Accept header
            })
            .AddXmlDataContractSerializerFormatters() // Add support for XML
            .AddJsonOptions(options => // Add support for JSON
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            services.AddDbContext<SharedContext>(options =>
                //options.UseNpgsql("Host=your_host;Database=your_db;Username=your_user;Password=your_password"))
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBL_Calculations, BL_Calculations>();
            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
