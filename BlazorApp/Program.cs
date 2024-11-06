using BlazorApp.Components;
using BlazorApp.Service;

namespace BlazorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Adds the neon.tech connection.
            /*builder.Services.AddSingleton(sp =>
            {
                // Goes into the configurations file and gets the connection string called "DefaultConnection".
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                return new DBService(connectionString);
            });*/

            // Adds the MongoDB connection.
            builder.Services.AddSingleton(sp =>
            {
                // Goes into the configurations file and gets the connection string called "DefaultConnection".
                var connectionString = builder.Configuration.GetConnectionString("MongoConnection");
                // Console.WriteLine($"Connection string Program.cs: {connectionString}");
                return new MongoDBService(connectionString);
            });
            
            builder.Services.AddSingleton<EvCar>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}