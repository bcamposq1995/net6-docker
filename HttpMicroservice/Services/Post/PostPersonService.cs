
using Commons.Models;
using HttpMicroservice.Repositories.Cache;
using HttpMicroservice.Repositories.Queue;

namespace People.HttpMicroservice.Services.Post
{
    public class PostPersonService : IPostPersonService
    {
        private readonly IQueueRepository _queueRepository;
        private readonly ICacheRepository _cacheRepository;

        public PostPersonService(IQueueRepository queueRepository, ICacheRepository cacheRepository)
        {
            this._cacheRepository = cacheRepository;
            this._queueRepository = queueRepository;
        }

        /// <summary>
        /// By post, it creates a new person, it does not validate if the person exists
        /// </summary>
        /// <param name="request">PostPersonRequest</param>
        /// <returns>PostPersonResponse</returns>
        public async Task<PostPersonResponse> Post(PostPersonRequest request)
        {
            PostPersonResponse response = new()
            {
                DateTime = DateTime.Now,
                Status = TransactionStatus.PENDING,
                TransactionId = Guid.NewGuid(),
                Request = request
            };

            await this._cacheRepository.SaveObject(response.TransactionId.ToString(), response, 24);
            this._queueRepository.Enqueue(response, "post");

            return response;
        }

        public async Task<PostPersonResponse> Transaction(Guid id) => (await this._cacheRepository.FindById<PostPersonResponse>(id))!;
    }
}

