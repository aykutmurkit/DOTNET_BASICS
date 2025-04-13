namespace Data.Seeding
{
    /// <summary>
    /// Seeder çalışma sıralarını merkezi olarak tanımlayan enum
    /// </summary>
    public enum SeederOrder
    {
        // Temel kullanıcı rolleri ve kullanıcılar (daha önce zaten eklenmiş olabilir)
        UserRoles = 1,
        Users = 2,
        
        // İstasyon ve platformlar
        Stations = 3,
        Platforms = 4,
        
        // Cihazlar
        Devices = 5,
        DeviceSettings = 6,
        DeviceStatuses = 7,
        
        // Mesajlar ve tahminler
        Predictions = 8,
        FullScreenMessages = 9,
        ScrollingScreenMessages = 10,
        BitmapScreenMessages = 11,
        PeriodicMessages = 12,
        
        // Zamanlanmış kurallar
        ScheduleRules = 13
    }
} 