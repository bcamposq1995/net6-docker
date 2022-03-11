namespace WorkerMicroservice.Services.Delete
{
	public interface IDeletePersonService
	{
		Task Delete(Guid id);
	}
}

