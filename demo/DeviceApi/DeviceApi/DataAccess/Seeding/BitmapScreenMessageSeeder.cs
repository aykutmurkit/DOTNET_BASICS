using Data.Context;
using Data.Seeding;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Bitmap ekran mesajları için örnek verileri ekler
    /// </summary>
    public class BitmapScreenMessageSeeder : ISeeder
    {
        // Order özelliği SeederOrder enum değeri kullanılarak tanımlandı
        public int Order => (int)SeederOrder.BitmapScreenMessages;
       
        /// <summary>
        /// Seed işlemini gerçekleştirir
        /// </summary>
        public async Task SeedAsync(AppDbContext context)
        {
            // Eğer BitmapScreenMessages tablosunda veri varsa çık
            if (await context.BitmapScreenMessages.AnyAsync())
            {
                return;
            }

            var queryBuilder = new StringBuilder();
            
            // IDENTITY_INSERT'i aç (ID'leri belirtebilmek için)
            queryBuilder.AppendLine("SET IDENTITY_INSERT [BitmapScreenMessages] ON;");
            
            // Örnek veriler
            queryBuilder.AppendLine(@"
INSERT INTO [BitmapScreenMessages] ([Id], [TurkishBitmap], [EnglishBitmap], [Duration], [CreatedAt], [UpdatedAt])
VALUES (1, 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAE0SURBVDjLlJLNSsNAFIWvL+ylhSwU2ii0YLMpvkBWSgVFfJyA+jIR8WfThZu0CLWC0L0go0KKXWiTcm5mHKFNahO/cOCeM/cm984k4nd/Pk+9o4qioDPvAKdBqLauSWqIwwoOfJCXXzXbOrJvxjgTTiJ0mzSKYFPADbuOPQgZG1mngNqS3QVZC/gZIeunmwJ68hMEt7o/HVlXsO/DOXvQrQM58HT6BZyHDmpVGVoHuN+PF0oXwIs9fCcLmqRwTzjiSZQSvPYgiB9cVQFRgZ86+dPiHzxFEU4XKXIVQzzcwMmkjsAzDF11o/Gg3QJYqCh9/YQ6aV+M2t2qOUdf3hhOgjZWHkDMNjPrMprebM8Hl7fzrxr31mNrJ8Ql7rPt8JovH62Hu9Wg8dGuH84HjuX7L//ID/SBfALrcagVkElGEwAAAABJRU5ErkJggg==', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAG7SURBVDjLpZM9S8NQFIYvpX9CK0hBg/MNdeqgi4g4dRAXRwc34+TgaKcILgVXwVVnJ8HBRReXIJY0aQcHQYtFi5jYpCFJm8/7npubtKZx8MDDPe/hnnPuPScqsSwLu1iqL/daNrGLbIwJYxQxRiRWkZGR7pHxae3i8h6TNxkWn1LEKGKMiEjZEHPxvD0t1i4vU2ijbyzQ/VnCkHFmInLMTwsRQU2MQvVPgZ0YEzuOQvHVwaM6Q+lFTsqFEJELgWkwr43CM7LwToPz23uFDYpJG2yNQ7X3MVwdg/4+AbWxC0OsCWrfIFmeuTUMYcA0YJXm+Y0Uzm4yEKMfJhNRBhMbmebhAw5jh9K0RsGMoXAewM+NDDzDQXWkWx3qCOimwxDNVJFizcgQciIiYW+D8QlTnEdkLMfBtGbQeIhvLU1I9ZFCcCpN6eFQq4Mw1cN9IX1znGLLPQpjcmVrQCK7gFPfqJhSI8/RjKXQerx+bTrnW4auXvqiWtmZVm/t6uUMHcitx2v/qFq+nebc1kLS6WjE8PEJGLwUwJ2aQP8+JTq4TG51+e8P2f/cu5XrQzE7eD787v0NPEixVZ4hRkIAAAAASUVORK5CYII=', 2, GETDATE(), NULL);

INSERT INTO [BitmapScreenMessages] ([Id], [TurkishBitmap], [EnglishBitmap], [Duration], [CreatedAt], [UpdatedAt])
VALUES (2, 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAIhSURBVDjLpZPLa1NREIe/+zSYRGJVEBERlyLS4sKFgpvufQHduNGFO/eC+Bf8B+7cCW5ELBQKIoKICCIiUiUYjTGaaJq0SfO695x7zuTHRULEWjpwmGEY5vvN9zyO894TpwZ+ZTwzg5OJXWINRyN7Bs9Fc0Pa+EIkjQZJ0DRL1NSHVBpWVpA2DIixSv8WCZ6jEqpqUXVPWse8i9isA069IcVBTUNKia9qiOluViHTJKqtElUb+CrABwnfF19ksUSSeAfZzpvbHB0pYaRCicLrL1PEds6qqtXr1BbmjG5P7OZ5f5nZ+QV8ncBkfThtGEmINdsEZXn9dc6rz1VY2kR2e5I70+e4dOQQUikO75nm+fwXhEiwkaHf9wzsnqDf9yhjSJEjmN2jRwnTvfyoLpBXISzX+Vld5mjvBC8W5mgoQ1JfQzrLlfGzGG1QfgAsZDv4GJJM1NA65cPXb5S0I44KJAxarZFa6mhrcGGbNDZorVn18zRaDTYTrcFUpGzG+rY2CcmRjp0QgkXbEgKrUQk4G0E0Yl4MxLgQiEJ4JfgYwMQaHxIsyVTZ/bhVWSJBWrDnHFu7ivR0dqFigHNYl7CBxlpLlCjyts1upfA+YMMOtqtD9O3YSS6OODF1FJvqKC7kKBR6SMqDDJYGsV6hYkNia1wS7v5JcVCWq+97+PjulsFCL9gMlEEFj+7Jcbx3F+ndx5//hL/fTr6vA/QUS8d/AHklmMpEJGnTAAAAAElFTkSuQmCC', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAHySURBVDjLtZPvT1JhGMbdX9PVlvrHrVuX/SE2WWuZLVuXMS0iEzGQUiuJXLqZkJaycA0EhBCk19JIQniNRDshHOCAgsifnNt5aSpNs2zJu95t33Xu5+u+n+fZFgsb2yRiEwuTSZWLJbVIF2VJVNREUSLSZUlUlGTK+VZRFFZmN9eCLtfLUMAP/W6JpC4R0gXC+gqhXdImX4KbPxecgU5u710P0vspTPY3wBbOwOY/xkT6Vaw2v4O079J35M9G8Us72KLvZmC2pEHJ25nkRyL9cLg8iKTTmCUFBMgXIJ8LsftT2NxqYm2ugdXQOTbWKlgOVpH7+gCro3d/CDgb1eIeHihQVqJY8DiQKxWxXioj/62AXLEAz5xAQqefRJaoae4yVYWsf1CnKFHQVOSVGtIXpyiUzvH0SQSPuygqfQOJHqJh1ZXYbXuT2hxVhSriCCWKypWKEE9W4Xx8Dk5mWc8z4CJJpzthIKzfpir5qsKHLRUcrU8hmDwHy/YWNK6sYee8bFv/tEFP2J5K5E861vY30X72GGxLGSRrKUQO15A6SsK5+ngB4Nw0xo5PwuKJtxEZgH2fRZLOI/1+Ge7Y0WT/iikwtC2zYFKoK5yw/qXL6J31EM68cGyO/wVcdw5IfDbaXwAAAABJRU5ErkJggg==', 3, GETDATE(), NULL);

INSERT INTO [BitmapScreenMessages] ([Id], [TurkishBitmap], [EnglishBitmap], [Duration], [CreatedAt], [UpdatedAt])
VALUES (3, 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAKFSURBVDjLhVNNaxNRFD3vzUwSJ622YEwgBetGIrjRB1IKLgUXgov+BZcKVhcVXPhZGaGbgCBiJG1dGarVYLSKCgVBxY1UQdDQJMY0H5OZee/d+zLTxEbahTdvuPfc8+67vzeb+iK48PFj5v+bw8wMK4oi/E8wvtsVJXO1VjIZ3xJoVTTEOGeMwXIcWJ4nLKcMq+NCjHOiLzfj2lUUB/OMVgdtVNs26kEAzhjjOQuxeFy0ORk4mQziZLtTlQZjwhtXAAXOgZh2uBgLS3CvTR/CiA5ZXMPCZ1epILKdCRRPXtcqCUAoACQc3RVLRlBJv8HeuXuIRMyoDWH+Pw8JJYSobANTLqQQwlDLBpQbm5Cp9uJmvw+5Q9PrCiCElAAgmCwLIYhQJDLprRI//jXpQb73AHwrVzG3awzPZqbXFYhkKlWMkn1X3rYUFqnCCRGiARCVdXo3DFkDxubnkMvlEI/HGwJRKpUaIvd9Pzzfw9rKCrzSEpy3zyG/T2NmJAnTNBurhAK+TnVWgY5HF2E0NUO68RYfLk/h46McpqamQE1tCES1Wm3UPwIzjQZjZR14fvgQcvmLCH0fiUQC2Wx2QyNtBCgUCnAcBz2792EqlUI6nUY4HGYbhkQEY7FYTCmtNmUymVOZTKYzlUo1G4ax6Pv+13A4vL2np6fXMIwWpdRmAF/y+fxrAK/6+vpq3f2js2y7yz6tVivl8/mx3t7eI8VisVtKaTDGvk1OTo4BEFLKxaGhod/0L+f2YIJHGfVHAAAAAElFTkSuQmCC', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAJFSURBVDjLpZNLaFNxEMa/0yT37qZJmia1GoyEULPGkhTcGFCo1G3VRcFWUaFVFy5cCLopCIJdxK4URRQEBfdh6UYpFHRRRHRhKzFVa9EarchDUm1uHv97c8/MNTRauim48PFj5v+bw8wMK4oi/E8wvtsVJXO1VjIZ3xJoVTTEOGeMwXIcWJ4nLKcMq+NCjHOiLzfj2lUUB/OMVgdtVNs26kEAzhjjOQuxeFy0ORk4mQziZLtTlQZjwhtXAAXOgZh2uBgLS3CvTR/CiA5ZXMPCZ1epILKdCRRPXtcqCUAoACQc3RVLRlBJv8HeuXuIRMyoDWH+Pw8JJYSobANTLqQQwlDLBpQbm5Cp9uJmvw+5Q9PrCiCElAAgmCwLIYhQJDLprRI//jXpQb73AHwrVzG3awzPZqbXFYhkKlWMkn1X3rYUFqnCCRGiARCVdXo3DFkDxubnkMvlEI/HGwJRKpUaIvd9Pzzfw9rKCrzSEpy3zyG/T2NmJAnTNBurhAK+TnVWgY5HF2E0NUO68RYfLk/h46McpqamQE1tCES1Wm3UPwIzjQZjZR14fvgQcvmLCH0fiUQC2Wx2QyNtBCgUCnAcBz2792EqlUI6nUY4HGYbhkQEY7FYTCmtNmUymVOZTKYzlUo1G4ax6Pv+13A4vL2np6fXMIwWpdRmAF/y+fxrAK/6+vpq3f2js2y7yz6tVivl8/mx3t7eI8VisVtKaTDGvk1OTo4BEFLKxaGhod/0L+f2YIJHGfVHAAAAAElFTkSuQmCC', 5, GETDATE(), NULL);
            ");
            
            // IDENTITY_INSERT'i kapat
            queryBuilder.AppendLine("SET IDENTITY_INSERT [BitmapScreenMessages] OFF;");
            
            // SQL komutunu çalıştır
            await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
            
            // Context'te cached entity'leri temizle
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
            
            // Cihazlara bitmap ekran mesajlarını atama
            try {
                // Mesajları belirli cihazlara ata
                var updateQuery = new StringBuilder();
                updateQuery.AppendLine("UPDATE [Devices] SET [BitmapScreenMessageId] = CASE");
                updateQuery.AppendLine("  WHEN [Id] % 3 = 1 THEN 1"); // ID'si 1, 4, 7, ... olan cihazlara 1. mesajı ata
                updateQuery.AppendLine("  WHEN [Id] % 3 = 2 THEN 2"); // ID'si 2, 5, 8, ... olan cihazlara 2. mesajı ata
                updateQuery.AppendLine("  WHEN [Id] % 3 = 0 THEN 3"); // ID'si 3, 6, 9, ... olan cihazlara 3. mesajı ata
                updateQuery.AppendLine("END");
                updateQuery.AppendLine("WHERE [Id] <= (SELECT COUNT(*) FROM [Devices])"); // Sadece mevcut cihazlar için
                
                // Güncelleme SQL komutunu çalıştır
                await context.Database.ExecuteSqlRawAsync(updateQuery.ToString());
                
                // Context'i tekrar temizle
                foreach (var entry in context.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Detached;
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Cihazlara BitmapScreenMessage atanırken hata: {ex.Message}");
            }
        }
    }
} 