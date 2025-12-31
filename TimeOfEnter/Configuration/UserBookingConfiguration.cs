using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TimeOfEnter.Configuration;

public class UserBookingConfiguration : IEntityTypeConfiguration<UserBooking>
{
    public void Configure(EntityTypeBuilder<UserBooking> builder)
    {
        builder.HasKey(ub => ub.Id);

        builder.HasOne(ub => ub.Date)
               .WithMany(d => d.Bookings)
               .HasForeignKey(ub => ub.DateId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ub => ub.User)
               .WithMany(u => u.Bookings)
               .HasForeignKey(ub => ub.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(ub => ub.UserId)
               .IsRequired();

        builder.Property(ub => ub.DateId)
               .IsRequired();

    }

}
