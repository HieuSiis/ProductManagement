using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
    public class LoginEmailViewModel
    {
		[Required]
		[EmailAddress]
		public string? Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}
}
