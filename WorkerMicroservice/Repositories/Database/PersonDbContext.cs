using Commons.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkerMicroservice.Repositories.Database
{
	public class PersonDbContext : DbContext
	{
		public PersonDbContext(DbContextOptions<PersonDbContext> options) : base(options) => this.Database.Migrate();

		public DbSet<Person>? People { get; set; }

	}
}

