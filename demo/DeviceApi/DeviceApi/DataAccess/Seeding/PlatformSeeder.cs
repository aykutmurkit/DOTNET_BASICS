using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Platformlar için seed data
    /// </summary>
    public class PlatformSeeder : ISeeder
    {
        // SeederOrder enum değerine göre ayarlandı
        public int Order => (int)SeederOrder.Platforms; // 4
        
        public async Task SeedAsync(AppDbContext context)
        {
            // Platformlar zaten varsa işlem yapma
            if (await context.Platforms.AnyAsync())
            {
                return;
            }
            
            // İstasyonları kontrol et
            var stations = await context.Stations.ToListAsync();
            if (!stations.Any())
            {
                throw new Exception("İstasyonlar bulunamadı. Önce StationSeeder çalıştırılmalıdır.");
            }

            // SQL komutu oluşturma ve yürütme
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Platforms] ON;");
            queryBuilder.AppendLine("INSERT INTO [Platforms] ([Id], [Latitude], [Longitude], [StationId]) VALUES");

            // Platform bilgileri - her istasyon için 2 platform (toplamda 62 platform)
            var platforms = new List<(int id, double latitude, double longitude, int stationId)>
            {
                // Yusufpaşa platformları (StationId: 1)
                (1, 41.0082, 28.9782, 1),
                (2, 41.0083, 28.9786, 1),
                
                // Kabataş platformları (StationId: 2)
                (3, 41.0331, 28.9949, 2),
                (4, 41.0333, 28.9951, 2),
                
                // Akşemsettin platformları (StationId: 3)
                (5, 41.0189, 28.9390, 3),
                (6, 41.0191, 28.9392, 3),
                
                // Gülhane platformları (StationId: 4)
                (7, 41.0138, 28.9810, 4),
                (8, 41.0140, 28.9812, 4),
                
                // Çapa-Şehremini platformları (StationId: 5)
                (9, 41.0085, 28.9354, 5),
                (10, 41.0087, 28.9356, 5),
                
                // Güneştepe platformları (StationId: 6)
                (11, 41.0222, 28.8910, 6),
                (12, 41.0224, 28.8912, 6),
                
                // Aksaray platformları (StationId: 7)
                (13, 41.0110, 28.9546, 7),
                (14, 41.0112, 28.9548, 7),
                
                // Tophane platformları (StationId: 8)
                (15, 41.0271, 28.9813, 8),
                (16, 41.0273, 28.9815, 8),
                
                // Güngören platformları (StationId: 9)
                (17, 41.0047, 28.8821, 9),
                (18, 41.0049, 28.8823, 9),
                
                // Zeytinburnu platformları (StationId: 10)
                (19, 40.9947, 28.9088, 10),
                (20, 40.9949, 28.9090, 10),
                
                // Sultanahmet platformları (StationId: 11)
                (21, 41.0056, 28.9773, 11),
                (22, 41.0058, 28.9775, 11),
                
                // Pazartekke platformları (StationId: 12)
                (23, 40.9981, 28.8950, 12),
                (24, 40.9983, 28.8952, 12),
                
                // Fındıkzade platformları (StationId: 13)
                (25, 41.0083, 28.9439, 13),
                (26, 41.0085, 28.9441, 13),
                
                // Çemberlitaş platformları (StationId: 14)
                (27, 41.0080, 28.9681, 14),
                (28, 41.0082, 28.9683, 14),
                
                // Sirkeci platformları (StationId: 15)
                (29, 41.0158, 28.9777, 15),
                (30, 41.0160, 28.9779, 15),
                
                // Bağcılar platformları (StationId: 16)
                (31, 41.0388, 28.8536, 16),
                (32, 41.0390, 28.8538, 16),
                
                // Laleli platformları (StationId: 17)
                (33, 41.0095, 28.9567, 17),
                (34, 41.0097, 28.9569, 17),
                
                // Soğanlı platformları (StationId: 18)
                (35, 41.0199, 28.8746, 18),
                (36, 41.0201, 28.8748, 18),
                
                // Merter Tekstil Sitesi platformları (StationId: 19)
                (37, 41.0063, 28.8942, 19),
                (38, 41.0065, 28.8944, 19),
                
                // Haseki platformları (StationId: 20)
                (39, 41.0065, 28.9396, 20),
                (40, 41.0067, 28.9398, 20),
                
                // Merkez Efendi platformları (StationId: 21)
                (41, 41.0000, 28.9144, 21),
                (42, 41.0002, 28.9146, 21),
                
                // Karaköy platformları (StationId: 22)
                (43, 41.0224, 28.9742, 22),
                (44, 41.0226, 28.9744, 22),
                
                // Eminönü platformları (StationId: 23)
                (45, 41.0174, 28.9741, 23),
                (46, 41.0176, 28.9743, 23),
                
                // Mithatpaşa platformları (StationId: 24)
                (47, 41.0050, 28.9196, 24),
                (48, 41.0052, 28.9198, 24),
                
                // Topkapı platformları (StationId: 25)
                (49, 41.0105, 28.9279, 25),
                (50, 41.0107, 28.9281, 25),
                
                // Yavuz Selim platformları (StationId: 26)
                (51, 41.0219, 28.9449, 26),
                (52, 41.0221, 28.9451, 26),
                
                // Fındıklı platformları (StationId: 27)
                (53, 41.0297, 28.9879, 27),
                (54, 41.0299, 28.9881, 27),
                
                // Mehmet Akif platformları (StationId: 28)
                (55, 41.0318, 28.8642, 28),
                (56, 41.0320, 28.8644, 28),
                
                // Beyazıt platformları (StationId: 29)
                (57, 41.0106, 28.9649, 29),
                (58, 41.0108, 28.9651, 29),
                
                // Akıncılar platformları (StationId: 30)
                (59, 41.0349, 28.8572, 30),
                (60, 41.0351, 28.8574, 30),
                
                // Cevizlibağ platformları (StationId: 31)
                (61, 41.0132, 28.9249, 31),
                (62, 41.0134, 28.9251, 31)
            };

            // Değerleri ekle
            for (int i = 0; i < platforms.Count; i++)
            {
                var (id, lat, lon, stationId) = platforms[i];
                queryBuilder.Append($"({id}, {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {stationId})");
                
                if (i < platforms.Count - 1)
                {
                    queryBuilder.AppendLine(",");
                }
                else
                {
                    queryBuilder.AppendLine(";");
                }
            }

            queryBuilder.AppendLine("SET IDENTITY_INSERT [Platforms] OFF;");

            try {
                // SQL komutunu çalıştır
                await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
                
                // Context cache'ini temizle
                foreach (var entry in context.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PlatformSeeder hatası: {ex.Message}");
                throw;
            }
        }
    }
} 