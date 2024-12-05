using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
	public class CartItem
	{
		public int? quantity { set; get; }
		public Product? product { set; get; }
		[Required]
		[EmailAddress]
		public string? Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

	}
}
