using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
	public class LoginUsernameViewModel
	{

		[Required]
		public string? UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string? Password { get; set; }
	}
}
