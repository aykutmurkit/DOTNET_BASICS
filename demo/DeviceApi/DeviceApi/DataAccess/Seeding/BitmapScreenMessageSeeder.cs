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
        /// <summary>
        /// Bu seeder'ın çalışma sırası (ScrollingScreenMessageSeeder'dan sonra çalışmalı)
        /// </summary>
        public int Order => 42; // ScrollingScreenMessageSeeder'dan hemen sonra

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
INSERT INTO [BitmapScreenMessages] ([Id], [TurkishBitmap], [EnglishBitmap], [CreatedAt], [UpdatedAt], [DeviceId])
VALUES (1, 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAE0SURBVDjLlJLNSsNAFIWvL+ylhSwU2ii0YLMpvkBWSgVFfJyA+jIR8WfThZu0CLWC0L0go0KKXWiTcm5mHKFNahO/cOCeM/cm984k4nd/Pk+9o4qioDPvAKdBqLauSWqIwwoOfJCXXzXbOrJvxjgTTiJ0mzSKYFPADbuOPQgZG1mngNqS3QVZC/gZIeunmwJ68hMEt7o/HVlXsO/DOXvQrQM58HT6BZyHDmpVGVoHuN+PF0oXwIs9fCcLmqRwTzjiSZQSvPYgiB9cVQFRgZ86+dPiHzxFEU4XKXIVQzzcwMmkjsAzDF11o/Gg3QJYqCh9/YQ6aV+M2t2qOUdf3hhOgjZWHkDMNjPrMprebM8Hl7fzrxr31mNrJ8Ql7rPt8JovH62Hu9Wg8dGuH84HjuX7L//ID/SBfALrcagVkElGEwAAAABJRU5ErkJggg==', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAG7SURBVDjLpZM9S8NQFIYvpX9CK0hBg/MNdeqgi4g4dRAXRwc34+TgaKcILgVXwVVnJ8HBRReXIJY0aQcHQYtFi5jYpCFJm8/7npubtKZx8MDDPe/hnnPuPScqsSwLu1iqL/daNrGLbIwJYxQxRiRWkZGR7pHxae3i8h6TNxkWn1LEKGKMiEjZEHPxvD0t1i4vU2ijbyzQ/VnCkHFmInLMTwsRQU2MQvVPgZ0YEzuOQvHVwaM6Q+lFTsqFEJELgWkwr43CM7LwToPz23uFDYpJG2yNQ7X3MVwdg/4+AbWxC0OsCWrfIFmeuTUMYcA0YJXm+Y0Uzm4yEKMfJhNRBhMbmebhAw5jh9K0RsGMoXAewM+NDDzDQXWkWx3qCOimwxDNVJFizcgQciIiYW+D8QlTnEdkLMfBtGbQeIhvLU1I9ZFCcCpN6eFQq4Mw1cN9IX1znGLLPQpjcmVrQCK7gFPfqJhSI8/RjKXQerx+bTrnW4auXvqiWtmZVm/t6uUMHcitx2v/qFq+nebc1kLS6WjE8PEJGLwUwJ2aQP8+JTq4TG51+e8P2f/cu5XrQzE7eD787v0NPEixVZ4hRkIAAAAASUVORK5CYII=', GETDATE(), NULL, 1);

INSERT INTO [BitmapScreenMessages] ([Id], [TurkishBitmap], [EnglishBitmap], [CreatedAt], [UpdatedAt], [DeviceId])
VALUES (2, 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAIhSURBVDjLpZPLa1NREIe/+zSYRGJVEBERlyLS4sKFgpvufQHduNGFO/eC+Bf8B+7cCW5ELBQKIoKICCIiUiUYjTGaaJq0SfO695x7zuTHRULEWjpwmGEY5vvN9zyO894TpwZ+ZTwzg5OJXWINRyN7Bs9Fc0Pa+EIkjQZJ0DRL1NSHVBpWVpA2DAixSv8WCZ6jEqpqUXVPWse8i9isA069IcVBTUNKia9qiOluViHTJKqtElUb+CrABwnfF19ksUSSeAfZzpvbHB0pYaRCicLrL1PEds6qqtXr1BbmjG5P7OZ5f5nZ+QV8ncBkfThtGEmINdsEZXn9dc6rz1VY2kR2e5I70+e4dOQQUikO75nm+fwXhEiwkaHf9wzsnqDf9yhjSJEjmN2jRwnTvfyoLpBXISzX+Vld5mjvBC8W5mgoQ1JfQzrLlfGzGG1QfgAsZDv4GJJM1NA65cPXb5S0I44KJAxarZFa6mhrcGGbNDZorVn18zRaDTYTrcFUpGzG+rY2CcmRjp0QgkXbEgKrUQk4G0E0Yl4MxLgQiEJ4JfgYwMQaHxIsyVTZ/bhVWSJBWrDnHFu7ivR0dqFigHNYl7CBxlpLlCjyts1upfA+YMMOtqtD9O3YSS6OODF1FJvqKC7kKBR6SMqDDJYGsV6hYkNia1wS7v5JcVCWq+97+PjulsFCL9gMlEEFj+7Jcbx3F+ndx5//hL/fTr6vA/QUS8d/AHklmMpEJGnTAAAAAElFTkSuQmCC', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAHySURBVDjLtZPvT1JhGMbdX9PVlvrHrVuX/SE2WWuZLVuXMS0iEzGQUiuJXLqZkJaycA0EhBCk19JIQniNRDshHOCAgsifnNt5aSpNs2zJu95t33Xu5+u+n+fZFgsb2yRiEwuTSZWLJbVIF2VJVNREUSLSZUlUlGTK+VZRFFZmN9eCLtfLUMAP/W6JpC4R0gXC+gqhXdImX4KbPxecgU5u710P0vspTPY3wBbOwOY/xkT6Vaw2v4O079J35M9G8Us72KLvZmC2pEHJ25nkRyL9cLg8iKTTmCUFBMgXIJ8LsftT2NxqYm2ugdXQOTbWKlgOVpH7+gCro3d/CDgb1eIeHihQVqJY8DiQKxWxXioj/62AXLEAz5xAQqefRJaoae4yVYWsf1CnKFHQVOSVGtIXpyiUzvH0SQSPuygqfQOJHqJh1ZXYbXuT2hxVhSriCCWKypWKEE9W4Xx8Dk5mWc8z4CJJpzthIKzfpir5qsKHLRUcrU8hmDwHy/YWNK6sYee8bFv/tEFP2J5K5E861vY30X72GGxLGSRrKUQO15A6SsK5+ngB4Nw0xo5PwuKJtxEZgH2fRZLOI/1+Ge7Y0WT/iikwtC2zYFKoK5yw/qXL6J31EM68cGyO/wVcdw5IfDbaXwAAAABJRU5ErkJggg==', GETDATE(), NULL, 2);

INSERT INTO [BitmapScreenMessages] ([Id], [TurkishBitmap], [EnglishBitmap], [CreatedAt], [UpdatedAt], [DeviceId])
VALUES (3, 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAKESURBVDjLnZPfa1JhGMf3RwjhTQi+N0FC6KIh3QxdlIVestDGYOiiKwldDB2MgekiR0Xe7Ef0MpKBVMiCUfZjzXmKkJlrTk2w9Yd0c5aKnp1zntfZOZ0XC8M5cLD3edB74P0874/neb9CAEK76z22Wq1BiqKapFIpzGQyllQqNefxeO4Ti8V63d3dVRXQW16v9xmOvLa21jYyMtJBEESD1Wp9uby8jHw+j2QyaalUKj1isXiLRCIx/fdG8/n840AgMGYwGECrN5hM96DXqWBx2GGz25DL5ZDJZNYikciQy+U62ZJAIqF9aZruxJbZIKxX6dHpmxpRGU+gULvQ1I+gsbGjqQJrH75hRmXHuVZrSdDS0jIyOzsLo9F4YmBgoCsMzOE3CNFV3Hw4DVupCFNmAbqFj5gJ5nCR6ED3mQttwusEh/f7+1EqlWriVgQ0Tc/Mzc1henoaVssSPPlveCcyQi9VYC6sI5jLI7Sigf5xDvLzZ9F5RvTz+Hm5PDw8DFEU66JWBPl8/pnBYBDu7GzA8CSFn/P3cSq1iCO+t1CNvkBRKcPwVgqlaRCiM/I6wRWVSgWz2SwoFAqwLMtkMpl6QTabDUs8DpPtE0KvluB+8xrZG9eR6+1BUa9D+cF9uJrPPmvWisA7ZGLSN9s/fqnP3dSV1FdZMqjp6GhUr6rAzWZdVFHcJwgQ9nqL6ov9R47K5dqIQnHj3o0BHB0HZcoV8NXWtEKhrI3FYucjkYgqFouZY7GYVa/XP+zt7U3ohkZRVipRUihwWSrF4OBgVbx94zJfz1eqt0KhEE6n0+j1epskyGdCMOsn0dTUBGUyOSgSiYrO0Qmw5+/19PQMrQEEnWCxn/+B5fHBu3UPm5ubNUaj8bRWq2V+AXBJbFkOOD69AAAAAElFTkSuQmCC', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAJRSURBVBgZpcHda81xHMDx9+d3fud3zrGF8xCiZpJYltwQUoiLFEkuKJJcKeVmZ7d2tyuKy1mKuN6/wIULSWlJXjc15iFb5mGcM9v5/r5fX7+DJJV8XovImE+0nyGiCd9CXD1gfe1y9dLC4tO0xaaTKjP2WNwtS/H+ATNimsrImE+0nyGiCd9CXD1g/dFNt/6Y7QQNpR1jYxNJp36rSzOn9i+c/33i0L5d0lT3cUVc2HYoZDzoYnPk9P5eAxOffjzjl7cJJJ2r23cuzcG/kPTsalzWbZ2aAhM7ixY1vpCMjD4cG5W9/vRp7zMtfQxFx7QPdziy9VB/TkxkzE3LZ5g4/87zQrzzs/Pie/FW2CLqnXm1yoi8UBaX1p0ZHDpiIYBWjKg6Wy/Yfnz/KggkS0vtI/cuXLj7TVklvRMmUqXYlIPJbCOLq2Rt5fPXcFbtXrcHyGddWVlf7Qyr5YZR5XRbAzjm9Saja4s4CThJJpthsbOPQjFEIUQxyRDAJJhpnYIKKMHCjLAI5bIPcC+BYVZteQsUSCnDYiwBmZpxMJ0iacVcS5VgVZSJKkmxUEQEEcWsQRQQAZMkxRgQz1AwM1ISA+RpxYiuFNNNyCGJ7/E8x0QVDYbGHhsS/mv+Y0KJcRJIYZ4oOTKZTHF1dW0NcurcuUu41psY9CXW9/6/hKJ329u8vv/i+J4sNe9zr8LNmzfuVlNTfQU4pqrQmIr5Jx0X9vtABdgNbAUagOa//R2Ap8AQcB24oKqf/wBDzrGpD5V/mwAAAABJRU5ErkJggg==', GETDATE(), NULL, 3);
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
        }
    }
} 