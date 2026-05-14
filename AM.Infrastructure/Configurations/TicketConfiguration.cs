using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AM.ApplicationCore.Domain;
namespace AM.Infrastructure.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t=> new {t.PassengerFK, t.FlightFK, t.NumTicket});
            builder.HasOne(t => t.Passenger)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.PassengerFK);
            builder.HasOne(t=>t.Flight)
                .WithMany(f=>f.Tickets)
                .HasForeignKey(t=>t.FlightFK);
        }
    }
}
