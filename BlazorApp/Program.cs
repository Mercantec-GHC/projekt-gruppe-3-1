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

            // The singleton
            builder.Services.AddSingleton(sp =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                return new DBService(connectionString);
            });
            
            // Added by Andreas-sama uWu~
            builder.Services.AddScoped<MiniCooper.BaseMiniCooper>();
            builder.Services.AddScoped<MiniCooper.EvMiniCooper>();
            builder.Services.AddScoped<MiniCooper.FossilMiniCooper>();
            builder.Services.AddScoped<MiniCooper.HybridMiniCooper>();
            builder.Services.AddScoped<MiniCooper.FullMiniCooper>();

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