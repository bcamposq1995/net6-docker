namespace Commons.Models
{
	public class HttpResponseException : Exception
    {
        public HttpResponseException(int statusCode, string message, Exception? ex = null) : base(message, ex) => (StatusCode) = (statusCode);

        public int StatusCode { get; }
    }
}

