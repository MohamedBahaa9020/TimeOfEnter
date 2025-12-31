using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TimeOfEnter.Database.Configuration;

public class DateConfiguration : IEntityTypeConfiguration<Date>
{
    public void Configure(EntityTypeBuilder<Date> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime);


        builder.HasIndex(x => x.IsActive);
    }
}