namespace LogLibrary.Configuration.Settings
{
    /// <summary>
    /// Loglama yapılandırma ayarları
    /// </summary>
    public class LogSettings
    {
        /// <summary>
        /// MongoDB bağlantı dizesi
        /// </summary>
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";
        
        /// <summary>
        /// MongoDB veritabanı adı
        /// </summary>
        public string DatabaseName { get; set; } = "LogsDb";
        
        /// <summary>
        /// MongoDB koleksiyon adı
        /// </summary>
        public string CollectionName { get; set; } = "Logs";
        
        /// <summary>
        /// Log kayıtlarının saklanma süresi (gün)
        /// </summary>
        public int RetentionDays { get; set; } = 30;
        
        /// <summary>
        /// Uygulama adı
        /// </summary>
        public string ApplicationName { get; set; } = "AuthApi";
        
        /// <summary>
        /// Çalışma ortamı
        /// </summary>
        public string Environment { get; set; } = "Development";
        
        /// <summary>
        /// Minimum log seviyesi (Trace, Debug, Information, Warning, Error, Critical)
        /// </summary>
        public string MinimumLevel { get; set; } = "Information";
        
        /// <summary>
        /// Hassas bilgilerin maskelenip maskelenmeyeceği
        /// </summary>
        public bool MaskSensitiveData { get; set; } = true;
        
        /// <summary>
        /// Log kaydetme işleminin asenkron olarak mı çalışacağı
        /// </summary>
        public bool EnableAsyncLogging { get; set; } = true;
        
        /// <summary>
        /// HTTP istek ve yanıtlarının loglanıp loglanmayacağı
        /// </summary>
        public bool EnableHttpLogging { get; set; } = true;
        
        /// <summary>
        /// GrayLog entegrasyonu etkinleştirilsin mi
        /// </summary>
        public bool EnableGraylog { get; set; } = false;
        
        /// <summary>
        /// GrayLog sunucu adresi
        /// </summary>
        public string GraylogHost { get; set; } = "localhost";
        
        /// <summary>
        /// GrayLog port numarası
        /// </summary>
        public int GraylogPort { get; set; } = 12201;
        
        /// <summary>
        /// ELK Stack entegrasyonu etkinleştirilsin mi
        /// </summary>
        public bool EnableElkStack { get; set; } = false;
        
        /// <summary>
        /// Elasticsearch URL'si
        /// </summary>
        public string ElasticsearchUrl { get; set; } = "http://localhost:9200";
        
        /// <summary>
        /// Elasticsearch indeks adı
        /// </summary>
        public string ElasticsearchIndex { get; set; } = "logs";
    }
} 