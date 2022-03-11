using System.ComponentModel.DataAnnotations;

namespace Commons.Models
{
	public class PostPersonRequest
	{
		[Required(ErrorMessage = "The first name field is required", AllowEmptyStrings = false)]
		[MaxLength(100, ErrorMessage = "The length of the first name field cannot exceed 100 characters")]
		public string? FirstName { get; set; }

		[Required(ErrorMessage = "The last name field is required", AllowEmptyStrings = false)]
		[MaxLength(100, ErrorMessage = "The length of the last name field cannot exceed 100 characters")]
		public string? LastName { get; set; }

		[Required(ErrorMessage = "The email field is required", AllowEmptyStrings = false)]
		[MaxLength(100, ErrorMessage = "The length of the email field cannot exceed 100 characters")]
		[EmailAddress(ErrorMessage = "The field email has an invalid email format")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "The birthday field is required")]
		public DateTime? Birthday { get; set; }
	}
}

