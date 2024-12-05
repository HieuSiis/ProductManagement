using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Models;

namespace ProductManagement.AppData
{
	public class AppDBContext : IdentityDbContext<AppUser>
	{
		public AppDBContext(DbContextOptions<AppDBContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

		}

		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<Order> Orders { get; set; } = null!;
		public DbSet<Category> Categories { get; set; } = null!;
		public DbSet<OrderProduct> OrderProducts { get; set; } = null!;
	}

}
