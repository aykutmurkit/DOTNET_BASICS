# TCP Listener

Bu modül, TCP bağlantıları dinlemek, istemcilerle iletişim kurmak ve belirli kurallara göre istekleri yanıtlamak için kullanılır.

## Özellikler

- 3456 portunda TCP bağlantı dinleme
- Her bağlantı için ayrı thread oluşturma
- İstek/yanıt (request/response) desenini uygulama
- DeviceApi ile entegrasyon
- Asenkron olmayan iletişim

## İstek/Yanıt Kuralları

TCP Listener'a gelen istekler aşağıdaki kurallara göre yanıtlanır:

- İstemci "8" gönderirse, yanıt "8" olur
- İstemci "7" gönderirse, yanıt "7" olur
- İstemci "5" gönderirse, yanıt "67" olur (hem 6 hem 7)
- Diğer tüm isteklerde, mesaj echo edilir (gelen mesaj aynen geri gönderilir)

## Test Etme

Hercules gibi TCP istemci araçları kullanarak 3456 portuna bağlanabilir ve mesajlar gönderebilirsiniz.

## Kullanım

Bu modül, DeviceApi'ye doğrudan entegre edilmiştir. Program.cs dosyasında otomatik olarak yüklenmektedir:

```csharp
// DeviceApi projesinde TCP Listener servisini ekleme
builder.Services.AddTcpListener(builder.Configuration);
```

## Ayarlar

TCP Listener modülü aşağıdaki ayarları kullanır:

- Port: TCP sunucusunun dinleyeceği port (varsayılan: 3456)
- IpAddress: TCP sunucusunun dinleyeceği IP adresi (varsayılan: 0.0.0.0)
- MaxConnections: Maksimum eşzamanlı bağlantı sayısı (varsayılan: 100)
- ConnectionTimeout: Bağlantı zaman aşımı süresi (milisaniye) (varsayılan: 30000)
- BufferSize: Buffer boyutu (byte) (varsayılan: 1024) 