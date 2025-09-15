using Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Configurations
{
    public class CustomerAttachmentConfiguration : IEntityTypeConfiguration<CustomerAttachment>
    {
        public void Configure(EntityTypeBuilder<CustomerAttachment> builder)
        {
            builder.HasKey(a => a.Id);

            builder
                .Property(a => a.CustomerId)
                .IsRequired();

            builder
                .Property(a => a.FileName)
                .IsRequired();

            builder
                .Property(a => a.ImageData)
                .HasColumnType("varbinary(max)")
                .IsRequired();
        }
    }
}
