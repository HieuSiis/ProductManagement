using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public string? ImagePath { get; set; }
		public int? CategoryId { get; set; }
		public Category? Category { get; set; }
		public ICollection<OrderProduct>? OrderProducts { get; set; }
		public int? Inventory {  get; set; }
		public int? Sold { get; set; }
		public decimal? Price { get; set; }
		public decimal? Discount { get; set; }
		public decimal? Tax { get; set; }

		[Timestamp]
		public DateTime CreatedAt { get; set; }

		[Timestamp]
		public DateTime UpdatedAt { get; set; }

	}
}

