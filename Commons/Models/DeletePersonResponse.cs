namespace Commons.Models
{
	public class DeletePersonResponse
	{
		public Guid TransactionId { get; set; }
		public TransactionStatus Status { get; set; }
		public Guid PersonId { get; set; }
		public WorkerResponseException? Error { get; set; }
		public DateTime DateTime { get; set; }
	}
}

