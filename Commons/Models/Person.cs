namespace Commons.Models
{
	public record Person(Guid Id)
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Email { get; set; }
		public DateTime? Birthday { get; set; }
	}
}

