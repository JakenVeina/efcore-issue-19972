using Microsoft.EntityFrameworkCore;

namespace EFCoreTest
{
    public class MyDbContext
        : DbContext
    {
        public const string ConnectionString
            = "Server=127.0.0.1;Port=5432;Database=EFCoreTest;User Id=postgres;Password=postgres;";

        public MyDbContext(
                DbContextOptions<MyDbContext> options)
            : base(
                options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
