using System;
namespace HttpMicroservice.Repositories.Queue
{
	public interface IQueueRepository
	{
		void Enqueue<T>(T obj, string queueName);
	}
}

