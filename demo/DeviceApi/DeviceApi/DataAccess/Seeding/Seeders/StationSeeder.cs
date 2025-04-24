using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// İstasyonlar için seed data
    /// </summary>
    public class StationSeeder : ISeeder
    {
        // SeederOrder enum değerine göre ayarlandı
        public int Order => (int)SeederOrder.Stations; // 3
        
        public async Task SeedAsync(AppDbContext context)
        {
            // İstasyonlar zaten varsa işlem yapma
            if (await context.Stations.AnyAsync())
            {
                return;
            }

            // SQL komutu oluşturma ve yürütme
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Stations] ON;");
            queryBuilder.AppendLine("INSERT INTO [Stations] ([Id], [Name], [Latitude], [Longitude]) VALUES");

            // T1 Hattı istasyonları
            var stations = new List<(int id, string name, double latitude, double longitude)>
            {
                (1, "Yusufpaşa", 41.0082, 28.9784),
                (2, "Kabataş", 41.0332, 28.9950),
                (3, "Akşemsettin", 41.0190, 28.9391),
                (4, "Gülhane", 41.0139, 28.9811),
                (5, "Çapa-Şehremini", 41.0086, 28.9355),
                (6, "Güneştepe", 41.0223, 28.8911),
                (7, "Aksaray", 41.0111, 28.9547),
                (8, "Tophane", 41.0272, 28.9814),
                (9, "Güngören", 41.0048, 28.8822),
                (10, "Zeytinburnu", 40.9948, 28.9089),
                (11, "Sultanahmet", 41.0057, 28.9774),
                (12, "Pazartekke", 40.9982, 28.8951),
                (13, "Fındıkzade", 41.0084, 28.9440),
                (14, "Çemberlitaş", 41.0081, 28.9682),
                (15, "Sirkeci", 41.0159, 28.9778),
                (16, "Bağcılar", 41.0389, 28.8537),
                (17, "Laleli", 41.0096, 28.9568),
                (18, "Soğanlı", 41.0200, 28.8747),
                (19, "Merter Tekstil Sitesi", 41.0064, 28.8943),
                (20, "Haseki", 41.0066, 28.9397),
                (21, "Merkez Efendi", 41.0001, 28.9145),
                (22, "Karaköy", 41.0225, 28.9743),
                (23, "Eminönü", 41.0175, 28.9742),
                (24, "Mithatpaşa", 41.0051, 28.9197),
                (25, "Topkapı", 41.0106, 28.9280),
                (26, "Yavuz Selim", 41.0220, 28.9450),
                (27, "Fındıklı", 41.0298, 28.9880),
                (28, "Mehmet Akif", 41.0319, 28.8643),
                (29, "Beyazıt", 41.0107, 28.9650),
                (30, "Akıncılar", 41.0350, 28.8573),
                (31, "Cevizlibağ", 41.0133, 28.9250)
            };

            // Değerleri ekle
            for (int i = 0; i < stations.Count; i++)
            {
                var (id, name, lat, lon) = stations[i];
                queryBuilder.Append($"({id}, '{name}', {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)})");
                
                if (i < stations.Count - 1)
                {
                    queryBuilder.AppendLine(",");
                }
                else
                {
                    queryBuilder.AppendLine(";");
                }
            }

            queryBuilder.AppendLine("SET IDENTITY_INSERT [Stations] OFF;");

            try
            {
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
                Console.WriteLine($"StationSeeder hatası: {ex.Message}");
                throw;
            }
        }
    }
} 