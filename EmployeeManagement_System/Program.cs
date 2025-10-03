using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Forms;
using EmployeeManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementSystem
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            // Set up Dependency Injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Initialize database with migration
            InitializeDatabase();

            // Windows Forms application configuration
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
        }

        static void ConfigureServices(ServiceCollection services)
        {
            // Add DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=TRYCATCHER;Database=EmployeeManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"));

            // Add custom services
            services.AddScoped<DatabaseService>();
        }

        static void InitializeDatabase()
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // This will create the database and apply any pending migrations
            context.Database.EnsureCreated();

            // Alternative: Use migrations (recommended for production)
            // context.Database.Migrate();
        }
    }
}