using LogLibrary.Configuration.Settings;
using LogLibrary.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace LogLibrary.Data.Context
{
    /// <summary>
    /// MongoDB database connection context for log storage.
    /// </summary>
    /// <remarks>
    /// Provides MongoDB connectivity with custom serialization for JObject data.
    /// Version: 1.0.0
    /// Author: R&amp;D Engineer Aykut Mürkit, İsbak
    /// </remarks>
    public class MongoDbContext
    {
        // Statik başlatıcı - sınıf ilk kullanıldığında bir kez çalışır
        static MongoDbContext()
        {
            // Global olarak MongoDB serileştirme ayarlarını yapılandır
            ConfigureMongoSerialization();
        }
        
        private readonly IMongoDatabase _database;
        private readonly LogSettings _logSettings;
        private readonly ILogger<MongoDbContext> _logger;
        private bool _isConnected = false;

        /// <summary>
        /// MongoDB bağlantısını başlatır
        /// </summary>
        public MongoDbContext(IOptions<LogSettings> logSettingsOptions, ILogger<MongoDbContext> logger = null)
        {
            _logSettings = logSettingsOptions.Value;
            _logger = logger;
            
            try
            {
                // Nesne düzeyinde serileştirme ayarlarını yapma - artık statik başlatıcıda yapılıyor
                // ConfigureMongoSerialization();
                
                if (string.IsNullOrEmpty(_logSettings.ConnectionString))
                {
                    LogError("MongoDB bağlantı dizesi boş. Varsayılan 'mongodb://localhost:27017' kullanılıyor.");
                    _logSettings.ConnectionString = "mongodb://localhost:27017";
                }
                
                if (string.IsNullOrEmpty(_logSettings.DatabaseName))
                {
                    LogError("MongoDB veritabanı adı boş. Varsayılan 'AuthApiLogs' kullanılıyor.");
                    _logSettings.DatabaseName = "AuthApiLogs";
                }
                
                if (string.IsNullOrEmpty(_logSettings.CollectionName))
                {
                    LogError("MongoDB koleksiyon adı boş. Varsayılan 'Logs' kullanılıyor.");
                    _logSettings.CollectionName = "Logs";
                }
                
                LogInfo($"MongoDB bağlantısı oluşturuluyor. ConnectionString: {MaskConnectionString(_logSettings.ConnectionString)}, Database: {_logSettings.DatabaseName}, Collection: {_logSettings.CollectionName}");
                
                var client = new MongoClient(_logSettings.ConnectionString);
                _database = client.GetDatabase(_logSettings.DatabaseName);
                
                // TTL indeksi oluştur - belirli bir süre sonra otomatik olarak kayıtları silmek için
                var collection = _database.GetCollection<LogEntry>(_logSettings.CollectionName);
                var indexKeysDefinition = Builders<LogEntry>.IndexKeys.Ascending(x => x.Timestamp);
                var indexOptions = new CreateIndexOptions 
                { 
                    ExpireAfter = TimeSpan.FromDays(_logSettings.RetentionDays) 
                };
                var indexModel = new CreateIndexModel<LogEntry>(indexKeysDefinition, indexOptions);
                collection.Indexes.CreateOne(indexModel);
                
                _isConnected = true;
                LogInfo("MongoDB bağlantısı başarıyla oluşturuldu.");
            }
            catch (MongoConfigurationException ex)
            {
                LogError($"MongoDB yapılandırma hatası: {ex.Message}");
                // Yapılandırma hatası olursa, bağlantı kurulmadan devam edebiliriz
                _isConnected = false;
            }
            catch (MongoConnectionException ex)
            {
                LogError($"MongoDB bağlantı hatası: {ex.Message}");
                // Bağlantı hatası olursa, bağlantı kurulmadan devam edebiliriz
                _isConnected = false;
            }
            catch (Exception ex)
            {
                LogError($"MongoDB hatası: {ex.Message}, StackTrace: {ex.StackTrace}");
                // Genel bir hata durumunda da uygulama çalışmaya devam etsin
                _isConnected = false;
            }
        }

        /// <summary>
        /// LogEntry koleksiyonunu döndürür, bağlantı yoksa null döner
        /// </summary>
        public IMongoCollection<LogEntry> LogEntries
        {
            get
            {
                if (!_isConnected || _database == null)
                {
                    LogWarning("MongoDB bağlantısı olmadığı için koleksiyona erişilemiyor.");
                    return null;
                }
                
                return _database.GetCollection<LogEntry>(_logSettings.CollectionName);
            }
        }
        
        /// <summary>
        /// MongoDB bağlantı durumunu kontrol eder
        /// </summary>
        public bool IsConnected => _isConnected && _database != null;
        
        #region Private Methods
        
        private void LogInfo(string message)
        {
            _logger?.LogInformation(message);
            Debug.WriteLine($"INFO: {message}");
        }
        
        private void LogWarning(string message)
        {
            _logger?.LogWarning(message);
            Debug.WriteLine($"WARNING: {message}");
        }
        
        private void LogError(string message)
        {
            _logger?.LogError(message);
            Debug.WriteLine($"ERROR: {message}");
            Console.WriteLine($"MongoDB Error: {message}");
        }
        
        private string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return string.Empty;
                
            try
            {
                // Kullanıcı adı ve şifre varsa maskeleme yapalım
                var uri = new MongoUrl(connectionString);
                if (!string.IsNullOrEmpty(uri.Username))
                {
                    return connectionString.Replace(uri.Username, "***").Replace(uri.Password, "***");
                }
            }
            catch
            {
                // Maskeleme sırasında hata olursa, orijinal değeri dönelim
            }
            
            return connectionString;
        }
        
        /// <summary>
        /// Configures MongoDB serialization settings globally.
        /// </summary>
        /// <remarks>
        /// Custom serializers for JObject and JArray types to handle serialization without type discriminators.
        /// Adds a custom convention to disable type information in MongoDB serialization.
        /// Version: 1.0.0
        /// Author: R&amp;D Engineer Aykut Mürkit, İsbak
        /// </remarks>
        private static void ConfigureMongoSerialization()
        {
            try
            {
                // Tip ayrımcısını tamamen devre dışı bırak
                BsonSerializer.RegisterSerializationProvider(new CustomSerializationProvider());
                
                // Convention'ları ayarla
                var pack = new ConventionPack
                {
                    new IgnoreExtraElementsConvention(true),
                    new IgnoreIfNullConvention(true)
                };
                ConventionRegistry.Register("CustomConventions", pack, t => true);
                
                // Tip diskriminatörlerini özel bir convention ile devre dışı bırak
                BsonSerializer.RegisterDiscriminatorConvention(
                    typeof(object), 
                    new DiscriminatorConvention()
                );
                
                // JObject türünü BsonDocument olarak serileştir
                if (!BsonClassMap.IsClassMapRegistered(typeof(JObject)))
                {
                    BsonSerializer.RegisterSerializer(typeof(JObject), new JObjectSerializer());
                }
                
                // JArray için özel serileştirici
                if (!BsonClassMap.IsClassMapRegistered(typeof(JArray)))
                {
                    BsonSerializer.RegisterSerializer(typeof(JArray), new JArraySerializer());
                }
                
                // LogEntry için açık bir sınıf haritası oluştur
                if (!BsonClassMap.IsClassMapRegistered(typeof(LogEntry)))
                {
                    BsonClassMap.RegisterClassMap<LogEntry>(cm =>
                    {
                        cm.AutoMap();
                        // Data alanı için özel serileştirici
                        cm.GetMemberMap(c => c.Data)
                          .SetSerializer(new BsonDocumentSerializer())
                          .SetIgnoreIfNull(true);
                    });
                }
                
                Console.WriteLine("MongoDB serileştirme ayarları global olarak yapılandırıldı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MongoDB serileştirme ayarları yapılandırılırken hata: {ex.Message}");
            }
        }
        
        /// <summary>
        /// JObject türünü MongoDB'ye serileştiren özel serileştirici
        /// </summary>
        private class JObjectSerializer : SerializerBase<JObject>
        {
            public override JObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                if (context.Reader.CurrentBsonType == BsonType.Null)
                {
                    context.Reader.ReadNull();
                    return null;
                }
                
                // Tip bilgisi yapısını kontrol et (_t/_v şeklinde)
                if (context.Reader.CurrentBsonType == BsonType.Document)
                {
                    var bookmark = context.Reader.GetBookmark();
                    context.Reader.ReadStartDocument();
                    
                    string elementName;
                    bool hasTypeDiscriminator = false;
                    bool hasValueField = false;
                    
                    // Dökümanın elemanlarını kontrol et
                    while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
                    {
                        elementName = context.Reader.ReadName();
                        
                        if (elementName == "_t")
                        {
                            hasTypeDiscriminator = true;
                            context.Reader.SkipValue(); // _t değerini atla
                        }
                        else if (elementName == "_v")
                        {
                            hasValueField = true;
                            if (context.Reader.CurrentBsonType == BsonType.Document)
                            {
                                var innerDocument = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
                                context.Reader.ReadEndDocument();
                                return JObject.Parse(innerDocument.ToJson());
                            }
                            else
                            {
                                context.Reader.SkipValue();
                            }
                        }
                        else
                        {
                            context.Reader.SkipValue();
                        }
                    }
                    
                    // Tip bilgisi bulunamadıysa başa dön ve normal belge olarak oku
                    if (!hasTypeDiscriminator || !hasValueField)
                    {
                        context.Reader.ReturnToBookmark(bookmark);
                    }
                    else
                    {
                        // Döküman sonunu oku ve null döndür (çünkü _v değeri okunamadı)
                        context.Reader.ReadEndDocument();
                        return new JObject();
                    }
                }
                
                var document = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
                return JObject.Parse(document.ToJson());
            }

            public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JObject value)
            {
                if (value == null)
                {
                    context.Writer.WriteNull();
                    return;
                }
                
                // JObject'i doğrudan BsonDocument olarak serialize et (tip bilgisi olmadan)
                var document = BsonDocument.Parse(value.ToString());
                BsonSerializer.Serialize(context.Writer, document);
            }
        }
        
        /// <summary>
        /// JArray türünü MongoDB'ye serileştiren özel serileştirici
        /// </summary>
        private class JArraySerializer : SerializerBase<JArray>
        {
            public override JArray Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                if (context.Reader.CurrentBsonType == BsonType.Null)
                {
                    context.Reader.ReadNull();
                    return null;
                }
                
                var array = BsonSerializer.Deserialize<BsonArray>(context.Reader);
                return JArray.Parse(array.ToJson());
            }

            public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JArray value)
            {
                if (value == null)
                {
                    context.Writer.WriteNull();
                    return;
                }
                
                var bsonArray = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(value.ToString());
                BsonSerializer.Serialize(context.Writer, bsonArray);
            }
        }
        
        /// <summary>
        /// CustomObjectSerializer - JObject ve dinamik türleri düzgün şekilde serileştiren özel serileştirici
        /// </summary>
        private class BsonDocumentSerializer : SerializerBase<object>
        {
            public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                if (context.Reader.CurrentBsonType == BsonType.Null)
                {
                    context.Reader.ReadNull();
                    return null;
                }

                // Tip bilgisi yapısını kontrol et (_t/_v şeklinde)
                if (context.Reader.CurrentBsonType == BsonType.Document)
                {
                    var bookmark = context.Reader.GetBookmark();
                    context.Reader.ReadStartDocument();
                    
                    string elementName;
                    bool hasTypeDiscriminator = false;
                    bool hasValueField = false;
                    
                    // Dökümanın elemanlarını kontrol et
                    while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
                    {
                        elementName = context.Reader.ReadName();
                        
                        if (elementName == "_t")
                        {
                            hasTypeDiscriminator = true;
                            string typeDiscriminator = context.Reader.ReadString();
                            
                            // Newtonsoft.Json.Linq.JObject için özel işlem
                            if (typeDiscriminator.Contains("JObject"))
                            {
                                // JObject'i beklemek için ileri sar
                                continue;
                            }
                            else
                            {
                                // Diğer tiplerde normal deserialize sürecine devam et
                                context.Reader.ReturnToBookmark(bookmark);
                                return BsonSerializer.Deserialize<BsonDocument>(context.Reader).ToBsonDocument();
                            }
                        }
                        else if (elementName == "_v")
                        {
                            hasValueField = true;
                            if (context.Reader.CurrentBsonType == BsonType.Document)
                            {
                                var innerDocument = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
                                context.Reader.ReadEndDocument();
                                return JObject.Parse(innerDocument.ToJson());
                            }
                            else
                            {
                                context.Reader.SkipValue();
                            }
                        }
                        else
                        {
                            context.Reader.SkipValue();
                        }
                    }
                    
                    // Tip bilgisi bulunamadıysa başa dön ve normal belge olarak oku
                    if (!hasTypeDiscriminator || !hasValueField)
                    {
                        context.Reader.ReturnToBookmark(bookmark);
                    }
                    else
                    {
                        // Döküman sonunu oku ve boş obje döndür
                        context.Reader.ReadEndDocument();
                        return new JObject();
                    }
                }
                
                // Diğer durumlar için normal BsonDocument deserialize et
                var document = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
                return JObject.Parse(document.ToJson());
            }

            public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
            {
                if (value == null)
                {
                    context.Writer.WriteNull();
                    return;
                }

                // JObject ise doğrudan objeyi serileştir
                if (value is JObject jObject)
                {
                    // JObject'i doğrudan BsonDocument olarak serialize et
                    var document = BsonDocument.Parse(jObject.ToString());
                    BsonSerializer.Serialize(context.Writer, document);
                    return;
                }
                
                // JArray ise doğrudan diziyi serileştir
                if (value is JArray jArray)
                {
                    var bsonArray = BsonSerializer.Deserialize<BsonArray>(jArray.ToString());
                    BsonSerializer.Serialize(context.Writer, bsonArray);
                    return;
                }

                // Diğer tipleri normal şekilde serileştir
                var serializer = BsonSerializer.LookupSerializer(value.GetType());
                serializer.Serialize(context, args, value);
            }
        }
        
        /// <summary>
        /// Tür ayrımcısını devre dışı bırakan özel bir serileştirme sağlayıcısı
        /// </summary>
        private class CustomSerializationProvider : IBsonSerializationProvider
        {
            public IBsonSerializer GetSerializer(Type type)
            {
                // JObject için özel serileştirici
                if (type == typeof(JObject))
                {
                    return new JObjectSerializer();
                }
                
                // JArray için özel serileştirici
                if (type == typeof(JArray))
                {
                    return new JArraySerializer();
                }
                
                return null;
            }
        }
        
        /// <summary>
        /// Tip diskriminatörlerini devre dışı bırakan özel convention
        /// </summary>
        private class DiscriminatorConvention : IDiscriminatorConvention
        {
            public string ElementName => "_t";

            public Type GetActualType(IBsonReader bsonReader, Type nominalType)
            {
                return nominalType;
            }

            public BsonValue GetDiscriminator(Type nominalType, Type actualType)
            {
                return null; // Tip bilgisi yazma
            }
        }
        
        #endregion
    }
} 