using System;
namespace Commons.Models
{
	public class PutPersonRequest
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Email { get; set; }
		public DateTime? Birthday { get; set; }
	}
}

