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
        
        // Mesajlar ve tahminler
        Predictions = 7,
        FullScreenMessages = 8,
        ScrollingScreenMessages = 9,
        BitmapScreenMessages = 10,
        PeriodicMessages = 11
    }
} 