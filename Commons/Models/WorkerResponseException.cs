namespace Commons.Models
{
	public class WorkerResponseException : Exception
    {
        public WorkerResponseException(int statusCode, string message, Exception? ex = null) : base(message, ex) => (StatusCode) = (statusCode);

        public int StatusCode { get; }
    }
}

