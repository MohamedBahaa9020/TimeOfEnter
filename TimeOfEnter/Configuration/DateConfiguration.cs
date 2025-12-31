using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TimeOfEnter.Configuration;

public class DateConfiguration : IEntityTypeConfiguration<Date>
{
    public void Configure(EntityTypeBuilder<Date> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.StartTime)
               .IsRequired();

        builder.Property(d => d.EndTime)
               .IsRequired();

        builder.Property(d => d.IsActive)
               .IsRequired();

        builder.HasIndex(d => d.IsActive);
    }
}
