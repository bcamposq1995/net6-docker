using Commons.Models;
using WorkerMicroservice.Repositories.Cache;
using WorkerMicroservice.Repositories.Database;

namespace WorkerMicroservice.Repositories.DataBase
{
    public class DatabaseRepository : IDatabaseRepository
    {

        private readonly PersonDbContext _dbContext;
        private readonly ICacheRepository _cache;

        public DatabaseRepository(PersonDbContext context, ICacheRepository cacheRepository)
        {
            this._dbContext = context;
            this._cache = cacheRepository;
        }

        /// <summary>
        /// Create a new person into the postres data base
        /// </summary>
        /// <param name="firstName">The person's first name</param>
        /// <param name="lastName">The person's last name</param>
        /// <param name="email">The person's email</param>
        /// <param name="birthday">The person's birthday</param>
        /// <returns>The person model created</returns>
        public async Task<Person> Create(string? firstName, string? lastName, string? email, DateTime? birthday)
        {
            var item = new Person(Guid.NewGuid())
            {
                Birthday = birthday,
                LastName = lastName,
                FirstName = firstName,
                Email = email
            };
            await this._dbContext.People!.AddAsync(item);
            await this._dbContext.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// Delete an existing person
        /// </summary>
        /// <param name="person">The person to delete</param>
        /// <returns>Nothing, this is an async function</returns>
        public async Task Delete(Person? person)
        {
            this._dbContext.Remove(person!);
            await this._dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Find a person by the id, returns null if it does not exist
        /// </summary>
        /// <param name="id">The person's id</param>
        /// <returns>The found person</returns>
        public async Task<Person> FindById(Guid id)
        {
            var objCache = await this._cache.FindById<Person>(id);
            if (objCache == null)
            {
                var objDB = await _dbContext.People!.FindAsync(id);
                await this._cache.SaveObject<Person>(id.ToString(), objDB!);
                return objDB!;
            }
            return objCache;
        }

        /// <summary>
        /// Filter data by the given parameters
        /// </summary>
        /// <param name="firstName">The person's first name</param>
        /// <param name="lastName">The person's last name</param>
        /// <param name="email">The person's email</param>
        /// <param name="birthday">The person's birthday</param>
        /// <returns></returns>
        public async Task<IEnumerable<Person>> List(string? firstName = null, string? lastName = null, string? email = null, DateTime? birthday = null)
        {
            var list = await this._cache.List<Person>();
            if(list == null || list.Count() == 0)
            {
                var query = this._dbContext.People!.AsQueryable();
                await this._cache.SaveObject<List<Person>>("list", query.ToList(), 1);
                if (firstName != null) query = query.Where(p => p.FirstName != null && p.FirstName.ToLower().Contains(firstName.ToLower()));
                if (lastName != null) query = query.Where(p => p.LastName != null && p.LastName.ToLower().Contains(lastName.ToLower()));
                if (email != null) query = query.Where(p => p.Email != null && p.Email.ToLower().Contains(email.ToLower()));
                if (birthday != null) query = query.Where(p => p.Birthday != null && p.Birthday == birthday);
                return query.ToList();
            }
            if (firstName != null) list = list.Where(p => p.FirstName != null && p.FirstName.ToLower().Contains(firstName.ToLower()));
            if (lastName != null) list = list.Where(p => p.LastName != null && p.LastName.ToLower().Contains(lastName.ToLower()));
            if (email != null) list = list.Where(p => p.Email != null && p.Email.ToLower().Contains(email.ToLower()));
            if (birthday != null) list = list.Where(p => p.Birthday != null && p.Birthday == birthday);
            return list;
        }

        /// <summary>
        /// Update a given person
        /// </summary>
        /// <param name="person">The person to be updated</param>
        /// <param name="firstName">The person's first name</param>
        /// <param name="lastName">The person's last name</param>
        /// <param name="email">The person's email</param>
        /// <param name="birthday">The person's birthday</param>
        /// <returns></returns>
        public async Task<Person> Update(Person person, string? firstName = null, string? lastName = null, string? email = null, DateTime? birthday = null)
        {
            if (firstName != null) person.FirstName = firstName;
            if (lastName != null) person.LastName = lastName;
            if (email != null) person.Email = email;
            if (birthday != null) person.Birthday = birthday;
            await this._dbContext.SaveChangesAsync();
            return person;
        }
    }
}

