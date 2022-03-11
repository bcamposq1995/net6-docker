using Commons.Models;
using WorkerMicroservice.Repositories.DataBase;

namespace WorkerMicroservice.Services.Post
{
    public class PostPersonService : IPostPersonService
    {
        private readonly IDatabaseRepository _dbRepository;

        public PostPersonService(IDatabaseRepository repository)
        {
            this._dbRepository = repository;
        }

        /// <summary>
        /// By post, it creates a new person, it does not validate if the person exists
        /// </summary>
        /// <param name="request">The post request</param>
        /// <returns>The person updated</returns>
        public async Task<Person> Post(PostPersonRequest request) =>
            await this._dbRepository.Create(firstName: request.FirstName,
                                            lastName: request.LastName,
                                            email: request.Email,
                                            birthday: request.Birthday);
    }
}

