using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Tren tahminleri için seed data
    /// </summary>
    public class PredictionSeeder : ISeeder
    {
        // Order özelliği SeederOrder enum değeri kullanılarak tanımlandı
        public int Order => (int)SeederOrder.Predictions;
        
        public async Task SeedAsync(AppDbContext context)
        {
            // Tahminler zaten varsa işlem yapma
            if (await context.Predictions.AnyAsync())
            {
                return;
            }

            // Önce platformları kontrol et
            var platforms = await context.Platforms.ToListAsync();
            if (!platforms.Any())
            {
                return; // Platformlar yoksa işlem yapma
            }

            // SQL komutu oluşturma
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Predictions] ON;");
            queryBuilder.AppendLine("INSERT INTO [Predictions] ([Id], [StationName], [Direction], [Train1], [Line1], [Destination1], [Time1], [Train2], [Line2], [Destination2], [Time2], [Train3], [Line3], [Destination3], [Time3], [ForecastGenerationAt], [CreatedAt], [PlatformId]) VALUES");

            // Şimdiki zaman ve tahmin zamanları için değerler
            var now = DateTime.Now;
            var forecastTime = now;
            
            // Tahmin örnek verileri - sadece Yusufpaşa, Kabataş ve Aksaray istasyonları için
            var predictions = new List<object>();
            
            // Yusufpaşa 1. platform için - 3 tren tahmini
            predictions.Add(new
            {
                Id = 1,
                StationName = "Yusufpaşa",
                Direction = "Kuzey",
                Train1 = "Express",
                Line1 = "T1",
                Destination1 = "Kabataş",
                Time1 = now.AddMinutes(5),
                Train2 = "Banliyö",
                Line2 = "T1",
                Destination2 = "Bağcılar",
                Time2 = now.AddMinutes(12),
                Train3 = "Hızlı",
                Line3 = "T1",
                Destination3 = "Eminönü",
                Time3 = now.AddMinutes(20),
                ForecastGenerationAt = forecastTime,
                CreatedAt = now,
                PlatformId = 1
            });
            
            // Yusufpaşa 2. platform için - 2 tren tahmini
            predictions.Add(new
            {
                Id = 2,
                StationName = "Yusufpaşa",
                Direction = "Güney",
                Train1 = "Express",
                Line1 = "T1",
                Destination1 = "Zeytinburnu",
                Time1 = now.AddMinutes(3),
                Train2 = "Banliyö",
                Line2 = "T1",
                Destination2 = "Topkapı",
                Time2 = now.AddMinutes(15),
                Train3 = (string)null,
                Line3 = (string)null,
                Destination3 = (string)null,
                Time3 = (DateTime?)null,
                ForecastGenerationAt = forecastTime,
                CreatedAt = now,
                PlatformId = 2
            });
            
            // Kabataş 1. platform için - 1 tren tahmini
            predictions.Add(new
            {
                Id = 3,
                StationName = "Kabataş",
                Direction = "Doğu",
                Train1 = "Express",
                Line1 = "T1",
                Destination1 = "Bağcılar",
                Time1 = now.AddMinutes(8),
                Train2 = (string)null,
                Line2 = (string)null,
                Destination2 = (string)null,
                Time2 = (DateTime?)null,
                Train3 = (string)null,
                Line3 = (string)null,
                Destination3 = (string)null,
                Time3 = (DateTime?)null,
                ForecastGenerationAt = forecastTime,
                CreatedAt = now,
                PlatformId = 3
            });
            
            // Aksaray platformu için - hiç tren tahmini yok
            predictions.Add(new
            {
                Id = 4,
                StationName = "Aksaray",
                Direction = "Batı",
                Train1 = (string)null,
                Line1 = (string)null,
                Destination1 = (string)null,
                Time1 = (DateTime?)null,
                Train2 = (string)null,
                Line2 = (string)null,
                Destination2 = (string)null,
                Time2 = (DateTime?)null,
                Train3 = (string)null,
                Line3 = (string)null,
                Destination3 = (string)null,
                Time3 = (DateTime?)null,
                ForecastGenerationAt = forecastTime,
                CreatedAt = now,
                PlatformId = 13
            });
            
            // SQL değerlerini oluştur
            for (int i = 0; i < predictions.Count; i++)
            {
                var p = predictions[i];
                dynamic prediction = p;
                
                queryBuilder.Append($"({prediction.Id}, '{prediction.StationName}', '{prediction.Direction}', ");
                
                // Nullable string değerleri
                queryBuilder.Append(prediction.Train1 != null ? $"'{prediction.Train1}', " : "NULL, ");
                queryBuilder.Append(prediction.Line1 != null ? $"'{prediction.Line1}', " : "NULL, ");
                queryBuilder.Append(prediction.Destination1 != null ? $"'{prediction.Destination1}', " : "NULL, ");
                
                // Nullable datetime değeri
                if (prediction.Time1 != null)
                {
                    var time1 = (DateTime)prediction.Time1;
                    queryBuilder.Append($"'{time1.ToString("yyyy-MM-dd HH:mm:ss")}', ");
                }
                else
                {
                    queryBuilder.Append("NULL, ");
                }
                
                // Train2 bilgileri
                queryBuilder.Append(prediction.Train2 != null ? $"'{prediction.Train2}', " : "NULL, ");
                queryBuilder.Append(prediction.Line2 != null ? $"'{prediction.Line2}', " : "NULL, ");
                queryBuilder.Append(prediction.Destination2 != null ? $"'{prediction.Destination2}', " : "NULL, ");
                
                // Time2 nullable datetime
                if (prediction.Time2 != null)
                {
                    var time2 = (DateTime)prediction.Time2;
                    queryBuilder.Append($"'{time2.ToString("yyyy-MM-dd HH:mm:ss")}', ");
                }
                else
                {
                    queryBuilder.Append("NULL, ");
                }
                
                // Train3 bilgileri
                queryBuilder.Append(prediction.Train3 != null ? $"'{prediction.Train3}', " : "NULL, ");
                queryBuilder.Append(prediction.Line3 != null ? $"'{prediction.Line3}', " : "NULL, ");
                queryBuilder.Append(prediction.Destination3 != null ? $"'{prediction.Destination3}', " : "NULL, ");
                
                // Time3 nullable datetime
                if (prediction.Time3 != null)
                {
                    var time3 = (DateTime)prediction.Time3;
                    queryBuilder.Append($"'{time3.ToString("yyyy-MM-dd HH:mm:ss")}', ");
                }
                else
                {
                    queryBuilder.Append("NULL, ");
                }
                
                // Zorunlu alanlar
                queryBuilder.Append($"'{prediction.ForecastGenerationAt.ToString("yyyy-MM-dd HH:mm:ss")}', ");
                queryBuilder.Append($"'{prediction.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")}', ");
                queryBuilder.Append($"{prediction.PlatformId})");
                
                if (i < predictions.Count - 1)
                {
                    queryBuilder.AppendLine(",");
                }
                else
                {
                    queryBuilder.AppendLine(";");
                }
            }
            
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Predictions] OFF;");
            
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
                Console.WriteLine($"PredictionSeeder hatası: {ex.Message}");
                throw;
            }
        }
    }
} 