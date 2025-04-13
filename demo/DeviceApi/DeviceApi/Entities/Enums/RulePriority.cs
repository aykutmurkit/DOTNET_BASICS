namespace Entities.Enums
{
    /// <summary>
    /// Zamanlanmış kurallar için öncelik seviyeleri
    /// </summary>
    public enum RulePriority
    {
        /// <summary>
        /// Düşük öncelik (Normal günlük kurallar)
        /// </summary>
        Low = 0,
        
        /// <summary>
        /// Orta öncelik (Özel gün kuralları)
        /// </summary>
        Medium = 1,
        
        /// <summary>
        /// Yüksek öncelik (Acil durum mesajları)
        /// </summary>
        High = 2
    }
} 