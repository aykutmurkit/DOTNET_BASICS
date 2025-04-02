using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// UserRole entity için yapılandırma sınıfı
    /// </summary>
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // Primary Key
            builder.HasKey(r => r.Id);
            
            // Id alanı yapılandırması - auto increment
            builder.Property(r => r.Id)
                  .UseIdentityColumn();
                  
            // Benzersiz indeksler
            builder.HasIndex(r => r.Name).IsUnique();
            
            // Diğer alan yapılandırmaları
            builder.Property(r => r.Name).HasMaxLength(50).IsRequired();
            
            // İlişkiler - UserRole'ün User ile olan ilişkisi User tarafında tanımlandı
        }
    }
} 