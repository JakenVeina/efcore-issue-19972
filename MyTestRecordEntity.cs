using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreTest
{
    public class MyTestRecordEntity
    {
        public MyTestRecordEntity(
            long id,
            int value,
            long ownerId)
        {
            Id = id;
            Value = value;
            OwnerId = ownerId;
        }

        public long Id { get; internal set; }

        public int Value { get; }

        public long OwnerId { get; }

        public MyTestUserEntity Owner { get; internal set; }
            = null!;
    }

    public class MyTestEntityConfigurator
        : IEntityTypeConfiguration<MyTestRecordEntity>
    {
        public void Configure(
            EntityTypeBuilder<MyTestRecordEntity> builder)
        {
            builder
                .ToTable("TestRecords");

            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .ValueGeneratedNever();

            builder
                .Property(x => x.Value);

            builder
                .HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId);
        }
    }
}
