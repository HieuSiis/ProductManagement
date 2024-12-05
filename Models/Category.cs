using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagement.Models
{
	public class Category
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public ICollection<Product>? Products { get; set; } = new List<Product>();

		[NotMapped] // Do not save to database
		public int ProductCount { get; set; }
	}
}
