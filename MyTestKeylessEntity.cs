
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreTest
{
    public class MyTestKeylessEntity
    {
        public MyTestKeylessEntity(
            long userId,
            int recordCount,
            long valueTotal)
        {
            UserId = userId;
            RecordCount = recordCount;
            ValueTotal = valueTotal;
        }

        public long UserId { get; }

        public int RecordCount { get; }

        public long ValueTotal { get; }
    }

    public class MyTestKeylessEntityConfigurator
        : IEntityTypeConfiguration<MyTestKeylessEntity>
    {
        public void Configure(
                EntityTypeBuilder<MyTestKeylessEntity> entityTypeBuilder)
        {
            entityTypeBuilder
                .HasNoKey();

            entityTypeBuilder
                .Property(x => x.UserId);

            entityTypeBuilder
                .Property(x => x.RecordCount);

            entityTypeBuilder
                .Property(x => x.ValueTotal);
        }
    }
}
