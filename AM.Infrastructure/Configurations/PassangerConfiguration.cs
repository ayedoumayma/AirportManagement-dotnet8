using AM.ApplicationCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Infrastructure.Configurations
{
    public class PassangerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder
                .HasDiscriminator<string>("IsTraveler")
                .HasValue<Traveller>("1")
                .HasValue<Staff>("2")
                .HasValue<Passenger>("0");
            builder.OwnsOne(p => p.FullName, fn =>
            {
                fn.Property(f => f.FirstName)
                .HasColumnName("PassFirstName")
                .HasMaxLength(30);
                fn.Property(f => f.LastName)
                .HasColumnName("PassLastName")
                .IsRequired();
            });
     
        }
    }
}
