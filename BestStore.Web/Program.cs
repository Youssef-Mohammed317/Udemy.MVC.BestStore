using BestStore.Application;
using BestStore.Application.Interfaces.Services;
using BestStore.Infrastructure;
using BestStore.Shared;
namespace BestStore.Web
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.Configure<PayPalSettings>(
                builder.Configuration.GetSection("PayPalSettings")
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider
                    .GetRequiredService<IDbInitializer>();

                await initializer.InitializeAsync();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();




            await app.RunAsync();
        }
    }
}
