using Data.Context;
using Entities.Concrete;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.Seeding
{
    /// <summary>
    /// Zamanlanmış kurallar için örnek veriler ekler
    /// </summary>
    public class ScheduleRuleSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext context)
        {
            // Zaten kurallar var mı kontrol et
            if (await context.ScheduleRules.AnyAsync())
            {
                return; // Eğer veri varsa işlem yapma
            }
            
            // Cihazları kontrol et
            var devices = await context.Devices.ToListAsync();
            if (devices.Count == 0)
            {
                throw new Exception("Cihazlar bulunamadı. Önce DeviceSeeder çalıştırılmalı.");
            }
            
            // FullScreenMessage var mı kontrol et
            var fullScreenMessages = await context.FullScreenMessages.ToListAsync();
            if (fullScreenMessages.Count == 0)
            {
                throw new Exception("Tam ekran mesajları bulunamadı. Önce FullScreenMessageSeeder çalıştırılmalı.");
            }
            
            // ScrollingScreenMessage var mı kontrol et
            var scrollingScreenMessages = await context.ScrollingScreenMessages.ToListAsync();
            if (scrollingScreenMessages.Count == 0)
            {
                throw new Exception("Kayan ekran mesajları bulunamadı. Önce ScrollingScreenMessageSeeder çalıştırılmalı.");
            }
            
            // BitmapScreenMessage var mı kontrol et
            var bitmapScreenMessages = await context.BitmapScreenMessages.ToListAsync();
            if (bitmapScreenMessages.Count == 0)
            {
                throw new Exception("Bitmap ekran mesajları bulunamadı. Önce BitmapScreenMessageSeeder çalıştırılmalı.");
            }
            
            // Örnek kurallar oluştur
            var scheduleRules = new List<ScheduleRule>();
            
            // Tüm cihazlar için günlük rutin kurallar
            foreach (var device in devices)
            {
                // Örnek 1: Hafta içi normal çalışma saati kuralı (Pazartesi-Cuma, 07:00-23:00)
                scheduleRules.Add(new ScheduleRule
                {
                    DeviceId = device.Id,
                    RuleName = "Hafta İçi Normal Çalışma",
                    StartDateTime = DateTime.Now.Date.AddHours(7), // Bugün 07:00
                    EndDateTime = DateTime.Now.Date.AddYears(1), // 1 yıl geçerli
                    IsRecurring = true,
                    RecurringDays = "1,2,3,4,5", // Pazartesi-Cuma
                    ScreenStatus = true, // Ekran açık
                    FullScreenMessageId = fullScreenMessages[0].Id,
                    ScrollingScreenMessageId = scrollingScreenMessages[0].Id,
                    BitmapScreenMessageId = bitmapScreenMessages[0].Id,
                    Priority = RulePriority.Low, // Düşük öncelik
                    Description = "Hafta içi normal çalışma saatleri için standart mesajları göster",
                    CreatedAt = DateTime.Now
                });
                
                // Örnek 2: Hafta sonu çalışma saati kuralı (Cumartesi-Pazar, 09:00-22:00)
                scheduleRules.Add(new ScheduleRule
                {
                    DeviceId = device.Id,
                    RuleName = "Hafta Sonu Çalışma",
                    StartDateTime = DateTime.Now.Date.AddHours(9), // Bugün 09:00
                    EndDateTime = DateTime.Now.Date.AddYears(1), // 1 yıl geçerli
                    IsRecurring = true,
                    RecurringDays = "6,7", // Cumartesi-Pazar
                    ScreenStatus = true, // Ekran açık
                    FullScreenMessageId = fullScreenMessages[1].Id,
                    ScrollingScreenMessageId = scrollingScreenMessages[1].Id,
                    BitmapScreenMessageId = bitmapScreenMessages[0].Id,
                    Priority = RulePriority.Low, // Düşük öncelik
                    Description = "Hafta sonu çalışma saatleri için standart mesajları göster",
                    CreatedAt = DateTime.Now
                });
            }
            
            // Özel Günler - Cumhuriyet Bayramı (Orta öncelikli)
            // İlk cihaz için örnek
            if (devices.Count > 0)
            {
                var firstDevice = devices[0];
                
                scheduleRules.Add(new ScheduleRule
                {
                    DeviceId = firstDevice.Id,
                    RuleName = "Cumhuriyet Bayramı Özel Mesajları",
                    StartDateTime = new DateTime(DateTime.Now.Year, 10, 29, 0, 0, 0), // 29 Ekim 00:00
                    EndDateTime = new DateTime(DateTime.Now.Year, 10, 29, 23, 59, 59), // 29 Ekim 23:59
                    IsRecurring = false, // Tek seferlik
                    RecurringDays = "0", // Tek seferlik, 0 değerini kullan
                    ScreenStatus = true, // Ekran açık
                    FullScreenMessageId = fullScreenMessages[2].Id,
                    ScrollingScreenMessageId = scrollingScreenMessages[2].Id,
                    BitmapScreenMessageId = bitmapScreenMessages[0].Id,
                    Priority = RulePriority.Medium, // Orta öncelik
                    Description = "Cumhuriyet Bayramı için özel mesajlar",
                    CreatedAt = DateTime.Now
                });
            }
            
            // Acil durum mesajı örneği (Yüksek öncelikli)
            // İkinci cihaz için örnek
            if (devices.Count > 1)
            {
                var secondDevice = devices[1];
                
                scheduleRules.Add(new ScheduleRule
                {
                    DeviceId = secondDevice.Id,
                    RuleName = "Acil Durum Mesajı",
                    StartDateTime = DateTime.Now.AddHours(-1), // 1 saat önce başla
                    EndDateTime = DateTime.Now.AddDays(1), // 1 gün sonra bitir
                    IsRecurring = false, // Tek seferlik
                    RecurringDays = "0", // Tek seferlik, 0 değerini kullan
                    ScreenStatus = true, // Ekran açık
                    FullScreenMessageId = fullScreenMessages[0].Id,
                    Priority = RulePriority.High, // Yüksek öncelik
                    Description = "Acil durum bildirimi",
                    CreatedAt = DateTime.Now
                });
            }
            
            // Her kuralın RecurringDays alanının null olmadığından emin olalım
            foreach (var rule in scheduleRules)
            {
                if (string.IsNullOrEmpty(rule.RecurringDays))
                {
                    // Eğer null veya boş ise, tekrarlamayan kurallar için "0" değerini ata
                    rule.RecurringDays = rule.IsRecurring ? "1,2,3,4,5,6,7" : "0";
                }
            }
            
            // Veritabanına ekle
            await context.ScheduleRules.AddRangeAsync(scheduleRules);
            await context.SaveChangesAsync();
        }
    }
} 