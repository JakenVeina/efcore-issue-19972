using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace EFCoreTest
{
    public static class Program
    {
        public const int UserCount = 10;
        public const int RecordCount = 1000;

        public static async Task Main()
        {
            using var rootServiceProvider = BuildServiceProvider();

            await ClearDatabaseAsync(
                rootServiceProvider);

            await SeedDatabaseAsync(
                rootServiceProvider,
                UserCount,
                RecordCount);

            await ReadKeylessEntitiesAsync(
                rootServiceProvider);
        }

        private static ServiceProvider BuildServiceProvider()
            => new ServiceCollection()
                .AddDbContext<MyDbContext>(optionsBuilder =>
                    optionsBuilder
                        .UseNpgsql(MyDbContext.ConnectionString))
                .AddLogging(loggingBuilder => loggingBuilder
                    .AddConsole())
                .BuildServiceProvider();

        private static async Task ClearDatabaseAsync(
            IServiceProvider rootServiceProvider)
        {
            using var serviceScope = rootServiceProvider.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<MyDbContext>();

            await dbContext.Database.ExecuteSqlRawAsync(@"DELETE FROM ""TestRecords""");
            await dbContext.Database.ExecuteSqlRawAsync(@"DELETE FROM ""TestUsers""");
        }

        private static async Task SeedDatabaseAsync(
            IServiceProvider rootServiceProvider,
            int userCount,
            int recordCount)
        {
            using var serviceScope = rootServiceProvider.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<MyDbContext>();

            dbContext.Set<MyTestUserEntity>()
                .AddRange(Enumerable.Range(1, userCount)
                    .Select(id => new MyTestUserEntity(
                        id,
                        $"User #{id}")));
            await dbContext.SaveChangesAsync();

            var rng = new Random(42);

            dbContext.Set<MyTestRecordEntity>()
                .AddRange(Enumerable.Range(1, recordCount)
                    .Select(id => new MyTestRecordEntity(
                        id,
                        rng.Next(),
                        rng.Next(1, userCount))));
            await dbContext.SaveChangesAsync();
        }

        private static async Task ReadKeylessEntitiesAsync(
            IServiceProvider rootServiceProvider)
        {
            using var serviceScope = rootServiceProvider.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<MyDbContext>();
            var logger = rootServiceProvider.GetRequiredService<ILogger<MyTestKeylessEntity>>();

            var query = dbContext.Set<MyTestKeylessEntity>()
                .FromSqlRaw(@"
                    SELECT
                        U.""Id""            AS ""UserId"",
                        COUNT(*)            AS ""RecordCount"",
                        SUM(R.""Value"")    AS ""ValueTotal""
                    FROM ""TestUsers"" U
                    JOIN ""TestRecords"" R
                        ON R.""OwnerId"" = U.""Id""
                    GROUP BY
                        U.""Id""
                    ORDER BY
                        U.""Id""")
                .AsAsyncEnumerable();

            await foreach(var keylessEntity in query)
                logger.LogInformation($"UserId: {keylessEntity.UserId,2}    RecordCount: {keylessEntity.RecordCount,4}  ValueTotal: {keylessEntity.ValueTotal}");
        }
    }
}
