using HRManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Api.Data.Configurations;

public sealed class PasswordResetRequestConfiguration
    : IEntityTypeConfiguration<PasswordResetRequest>
{
    public void Configure(EntityTypeBuilder<PasswordResetRequest> builder)
    {
        builder.HasKey(request => request.Id);

        builder.Property(request => request.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(request => request.UserId)
            .IsUnique()
            .HasFilter("status = 'Pending' AND NOT is_deleted");

        builder.HasOne(request => request.User)
            .WithMany()
            .HasForeignKey(request => request.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
