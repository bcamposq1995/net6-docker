namespace Commons.Models
{
	public class PutPersonResponse
	{
		public Guid TransactionId { get; set; }
		public TransactionStatus Status { get; set; }
		public PutPersonRequest? Request { get; set; }
		public Guid PersonId { get; set; }
		public Person? Response { get; set; }
		public WorkerResponseException? Error { get; set; }
		public DateTime DateTime { get; set; }
	}
}

