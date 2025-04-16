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
            builder.Property(ds => ds.ServerIp).HasMaxLength(50).IsRequired();
            builder.Property(ds => ds.TcpPort).IsRequired();
            builder.Property(ds => ds.UdpPort).IsRequired();
            builder.Property(ds => ds.FtpStatus).IsRequired();
            
            // Yeni eklenen alanlar
            builder.Property(ds => ds.FtpIp).HasMaxLength(50);
            builder.Property(ds => ds.FtpPort);
            builder.Property(ds => ds.FtpUsername).HasMaxLength(100);
            builder.Property(ds => ds.FtpPassword).HasMaxLength(100);
            builder.Property(ds => ds.ConnectionTimeoutDuration);
            builder.Property(ds => ds.CommunicationHardwareVersion).HasMaxLength(50);
            builder.Property(ds => ds.CommunicationSoftwareVersion).HasMaxLength(50);
            builder.Property(ds => ds.GraphicsCardHardwareVersion).HasMaxLength(50);
            builder.Property(ds => ds.GraphicsCardSoftwareVersion).HasMaxLength(50);
            builder.Property(ds => ds.ScrollingTextSpeed);
            builder.Property(ds => ds.TramDisplayType).HasMaxLength(50);
            builder.Property(ds => ds.BusScreenPageCount);
            builder.Property(ds => ds.TimeDisplayFormat).HasMaxLength(50);
            builder.Property(ds => ds.TramFont).HasMaxLength(50);
            builder.Property(ds => ds.ScreenVerticalPixelCount);
            builder.Property(ds => ds.ScreenHorizontalPixelCount);
            builder.Property(ds => ds.TemperatureAlarmThreshold);
            builder.Property(ds => ds.HumidityAlarmThreshold);
            builder.Property(ds => ds.GasAlarmThreshold);
            builder.Property(ds => ds.LightSensorStatus);
            builder.Property(ds => ds.LightSensorOperationLevel);
            builder.Property(ds => ds.LightSensorLevel1);
            builder.Property(ds => ds.LightSensorLevel2);
            builder.Property(ds => ds.LightSensorLevel3);
            builder.Property(ds => ds.SocketType).HasMaxLength(50);
            builder.Property(ds => ds.StopName).HasMaxLength(100);
            builder.Property(ds => ds.StartupLogoFilename).HasMaxLength(255);
            builder.Property(ds => ds.StartupLogoCrc16).HasMaxLength(50);
            builder.Property(ds => ds.VehicleLogoFilename).HasMaxLength(255);
            builder.Property(ds => ds.VehicleLogoCrc16).HasMaxLength(50);
            builder.Property(ds => ds.CommunicationType).HasMaxLength(50);
            
            // Device ile one-to-one ilişki
            builder.HasOne(ds => ds.Device)
                .WithOne(d => d.Settings)
                .HasForeignKey<DeviceSettings>(ds => ds.DeviceId)
                .OnDelete(DeleteBehavior.Cascade); // Device silinince DeviceSettings de silinecek
        }
    }
} 