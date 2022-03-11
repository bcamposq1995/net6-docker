namespace Commons.Models
{
	public class PatchPersonResponse
	{
		public Guid TransactionId { get; set; }
		public TransactionStatus Status { get; set; }
		public PatchPersonRequest? Request { get; set; }
		public Person? Response { get; set; }
		public Guid PersonId { get; set; }
		public WorkerResponseException? Error { get; set; }
		public DateTime DateTime { get; set; }
	}
}

