namespace Entities.Dtos
{
    /// <summary>
    /// Tren tahmin veri transfer nesnesi
    /// </summary>
    public class PredictionDto
    {
        public int Id { get; set; }
        public string StationName { get; set; }
        public string Direction { get; set; }
        
        // İlk tren tahmini (null olabilir)
        public string Train1 { get; set; }
        public string Line1 { get; set; }
        public string Destination1 { get; set; }
        public DateTime? Time1 { get; set; }
        
        // İkinci tren tahmini (null olabilir)
        public string Train2 { get; set; }
        public string Line2 { get; set; }
        public string Destination2 { get; set; }
        public DateTime? Time2 { get; set; }
        
        // Üçüncü tren tahmini (null olabilir)
        public string Train3 { get; set; }
        public string Line3 { get; set; }
        public string Destination3 { get; set; }
        public DateTime? Time3 { get; set; }
        
        // Tahmin oluşturma ve kayıt tarihi
        public DateTime ForecastGenerationAt { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // İlişkili platform ID'si
        public int PlatformId { get; set; }
    }

    /// <summary>
    /// Tahmin oluşturma isteği
    /// </summary>
    public class CreatePredictionRequest
    {
        public string StationName { get; set; }
        public string Direction { get; set; }
        
        public string Train1 { get; set; }
        public string Line1 { get; set; }
        public string Destination1 { get; set; }
        public DateTime? Time1 { get; set; }
        
        public string Train2 { get; set; }
        public string Line2 { get; set; }
        public string Destination2 { get; set; }
        public DateTime? Time2 { get; set; }
        
        public string Train3 { get; set; }
        public string Line3 { get; set; }
        public string Destination3 { get; set; }
        public DateTime? Time3 { get; set; }
    }
    
    /// <summary>
    /// Tahmin güncelleme isteği
    /// </summary>
    public class UpdatePredictionRequest
    {
        public string StationName { get; set; }
        public string Direction { get; set; }
        
        public string Train1 { get; set; }
        public string Line1 { get; set; }
        public string Destination1 { get; set; }
        public DateTime? Time1 { get; set; }
        
        public string Train2 { get; set; }
        public string Line2 { get; set; }
        public string Destination2 { get; set; }
        public DateTime? Time2 { get; set; }
        
        public string Train3 { get; set; }
        public string Line3 { get; set; }
        public string Destination3 { get; set; }
        public DateTime? Time3 { get; set; }
    }
} 