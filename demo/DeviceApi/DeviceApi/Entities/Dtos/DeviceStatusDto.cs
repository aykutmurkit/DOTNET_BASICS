namespace Entities.Dtos
{
    /// <summary>
    /// Cihaz durum bilgisi transfer nesnesi
    /// </summary>
    public class DeviceStatusDto
    {
        public int Id { get; set; }
        
        public bool FullScreenMessageStatus { get; set; }
        
        public bool ScrollingScreenMessageStatus { get; set; }
        
        public bool BitmapScreenMessageStatus { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public int DeviceId { get; set; }
        
        public string DeviceName { get; set; }
    }
    
    /// <summary>
    /// Cihaz durum oluşturma isteği transfer nesnesi
    /// </summary>
    public class CreateDeviceStatusDto
    {
        public bool FullScreenMessageStatus { get; set; }
        
        public bool ScrollingScreenMessageStatus { get; set; }
        
        public bool BitmapScreenMessageStatus { get; set; }
        
        public int DeviceId { get; set; }
    }
    
    /// <summary>
    /// Cihaz durum güncelleme isteği transfer nesnesi
    /// </summary>
    public class UpdateDeviceStatusDto
    {
        public bool FullScreenMessageStatus { get; set; }
        
        public bool ScrollingScreenMessageStatus { get; set; }
        
        public bool BitmapScreenMessageStatus { get; set; }
        
        public int DeviceId { get; set; }
    }
} 