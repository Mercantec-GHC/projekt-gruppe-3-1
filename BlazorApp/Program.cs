using BlazorApp.Components;
using BlazorApp.Service;
using Npgsql;

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

            // The singleton
            builder.Services.AddSingleton(sp =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                if (connectionString == null)
                {
                    Console.WriteLine("Connection string is null.");
                    return null;
                }
                
                return new DBService(connectionString);
            });
            
            builder.Services.AddScoped<MiniCooper.BaseMiniCooper>();
            builder.Services.AddScoped<MiniCooper.EvMiniCooper>();
            builder.Services.AddScoped<MiniCooper.FossilMiniCooper>();
            builder.Services.AddScoped<MiniCooper.HybridMiniCooper>();
            builder.Services.AddScoped<MiniCooper.FullMiniCooper>();
            builder.Services.AddScoped<UsersService.User>();
            builder.Services.AddScoped<MiniCooper.FullMiniCoopersState>();

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