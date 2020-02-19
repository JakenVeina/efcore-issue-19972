using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreTest
{
    public class MyTestUserEntity
    {
        public MyTestUserEntity(
            long id,
            string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; internal set; }

        public string Name { get; }
    }

    public class MyTestUserEntityConfigurator
        : IEntityTypeConfiguration<MyTestUserEntity>
    {
        public void Configure(
            EntityTypeBuilder<MyTestUserEntity> builder)
        {
            builder
                .ToTable("TestUsers");

            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .ValueGeneratedNever();

            builder
                .Property(x => x.Name);
        }
    }
}
