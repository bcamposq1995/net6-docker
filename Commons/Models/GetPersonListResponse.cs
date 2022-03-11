namespace Commons.Models
{
	public class GetPersonListResponse
	{
		public Guid TransactionId { get; set; }
		public TransactionStatus Status { get; set; }
		public List<Person>? Response { get; set; }
		public WorkerResponseException? Error { get; set; }
		public DateTime DateTime { get; set; }
	}
}

