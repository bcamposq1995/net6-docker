
namespace Commons.Models
{
	public class PostPersonResponse
	{
		public Guid TransactionId { get; set; }
		public TransactionStatus Status { get; set; }
		public PostPersonRequest? Request { get; set; }
		public Person? Response { get; set; }
		public WorkerResponseException? Error { get; set; }
		public DateTime DateTime { get; set; }
	}
}

