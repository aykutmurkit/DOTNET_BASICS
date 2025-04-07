using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// DeviceSettings entity konfigürasyonu
    /// </summary>
    public class DeviceSettingsConfiguration : IEntityTypeConfiguration<DeviceSettings>
    {
        public void Configure(EntityTypeBuilder<DeviceSettings> builder)
        {
            // Temel alanlar konfigürasyonu
            builder.HasKey(ds => ds.Id);
            builder.Property(ds => ds.ApnName).HasMaxLength(100).IsRequired();
            builder.Property(ds => ds.ApnUsername).HasMaxLength(100);
            builder.Property(ds => ds.ApnPassword).HasMaxLength(100);
            builder.Property(ds => ds.ServerIP).HasMaxLength(50).IsRequired();
            builder.Property(ds => ds.TcpPort).IsRequired();
            builder.Property(ds => ds.UdpPort).IsRequired();
            builder.Property(ds => ds.FtpStatus).IsRequired();
            
            // Device ile one-to-one ilişki
            builder.HasOne(ds => ds.Device)
                .WithOne(d => d.Settings)
                .HasForeignKey<DeviceSettings>(ds => ds.DeviceId)
                .OnDelete(DeleteBehavior.Cascade); // Device silinince DeviceSettings de silinecek
        }
    }
} 