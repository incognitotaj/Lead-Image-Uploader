using Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(a => a.Id);

            builder
                .Property(a => a.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder
                .Property(a => a.Email)
                .IsRequired();

            builder
                .HasMany(a => a.Attachments)
                .WithOne(a => a.Customer)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
