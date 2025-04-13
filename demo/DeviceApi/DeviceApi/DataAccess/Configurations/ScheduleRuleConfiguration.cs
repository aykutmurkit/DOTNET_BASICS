using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// Zamanlanmış kurallar için veritabanı yapılandırması
    /// </summary>
    public class ScheduleRuleConfiguration : IEntityTypeConfiguration<ScheduleRule>
    {
        public void Configure(EntityTypeBuilder<ScheduleRule> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.RuleName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(e => e.StartDateTime)
                .IsRequired();
                
            builder.Property(e => e.EndDateTime)
                .IsRequired();
                
            builder.Property(e => e.RecurringDays)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("0"); // "0" for non-recurring rules
                
            builder.Property(e => e.Description)
                .HasMaxLength(500);
                
            builder.Property(e => e.CreatedAt)
                .IsRequired();
                
            // Cihaz ilişkisi
            builder.HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 