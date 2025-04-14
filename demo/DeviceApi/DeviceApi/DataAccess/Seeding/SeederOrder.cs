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
        
        // Temel sabit değerler ve enum türleri
        AlignmentTypes = 5,
        FontTypes = 6,
        
        // Cihazlar
        Devices = 7,
        DeviceSettings = 8,
        DeviceStatuses = 9,
        
        // Mesajlar ve tahminler
        Predictions = 10,
        FullScreenMessages = 11,
        ScrollingScreenMessages = 12,
        BitmapScreenMessages = 13,
        PeriodicMessages = 14
    }
} 