namespace DeviceApi.API.Models
{
    /// <summary>
    /// Enum değerleri için DTO sınıfı
    /// </summary>
    public class EnumValueDto
    {
        /// <summary>
        /// Enum değerinin ID'si (veritabanı ID'si)
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Enum değerinin anahtar değeri (enum'daki sayısal değere benzer)
        /// </summary>
        public int Key { get; set; }
        
        /// <summary>
        /// Enum değerinin adı
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Enum değerinin açıklaması (varsa)
        /// </summary>
        public string Description { get; set; }
    }
} 