using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductManagement.AppData;
using ProductManagement.Data;
namespace ProductManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

			//builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));
			builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(connectionString));


			//builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AppDBContext>();
			builder.Services.AddDefaultIdentity<AppUser>(options => options.User.RequireUniqueEmail = true).AddEntityFrameworkStores<AppDBContext>();

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// Thêm các dịch vụ vào DI container
			builder.Services.AddDistributedMemoryCache();   // Đăng ký dịch vụ lưu cache trong bộ nhớ

			builder.Services.AddSession(cfg => {            // Đăng ký dịch vụ Session
				cfg.Cookie.Name = "ProductManagement";      // Đặt tên Session
				cfg.IdleTimeout = new TimeSpan(0, 30, 0);   // Thời gian tồn tại của Session
			});

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

			app.UseSession();   // Sử dụng Session

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
			
		    //app.MapRazorPages();
			app.Run();
        }
    }
}
