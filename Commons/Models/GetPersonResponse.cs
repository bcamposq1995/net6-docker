namespace Commons.Models
{
	public class GetPersonResponse
	{
		public Guid TransactionId { get; set; }
		public TransactionStatus Status { get; set; }
		public Guid IdPerson { get; set; }
		public Person? Response { get; set; }
		public WorkerResponseException? Error { get; set; }
		public DateTime DateTime { get; set; }
	}
}

