# TCP Listener Model Yapılandırması

TCP Listener modülünde kullanılan model sınıfları, üç ana kategoride düzenlenmiştir:

## 1. Protocol

`DeviceApi.TCPListener.Models.Protocol` namespace'i altında, haberleşme protokolüne özgü model sınıfları bulunur:

- **MessageType**: Mesaj tiplerini tanımlayan enum (Handshake, OpeningScreen, Settings, vb.)
- **CommunicationType**: İletişim tiplerini tanımlayan enum (TcpIp, Gsm, vb.)

## 2. Devices

`DeviceApi.TCPListener.Models.Devices` namespace'i altında, cihazlarla ilgili model sınıfları bulunur:

- **DeviceMessage**: Cihazdan gelen mesajları temsil eden sınıf
- **DeviceResponse**: Cihazlara gönderilecek yanıtları temsil eden sınıf
- **DeviceApprovalStatus**: Cihaz onay durumunu belirten enum
- **ResponseCode**: Yanıt kodlarını belirten enum
- **ApprovedDeviceDto**: Onaylı cihaz bilgilerini içeren DTO
- **UnapprovedDeviceDto**: Onaylanmamış cihaz bilgilerini içeren DTO

## 3. Configuration

`DeviceApi.TCPListener.Models.Configuration` namespace'i altında, yapılandırma ile ilgili model sınıfları bulunur:

- **TcpListenerSettings**: TCP Listener ayarlarını tanımlayan sınıf (Port, IpAddress, MaxConnections, vb.)

## Kullanım

Örneğin, bir cihaz mesajını işlemek için:

```csharp
using DeviceApi.TCPListener.Models.Protocol;
using DeviceApi.TCPListener.Models.Devices;

// Mesaj oluştur
var message = new DeviceMessage
{
    MessageType = MessageType.Handshake,
    Imei = "123456789012345",
    CommunicationType = CommunicationType.TcpIp
};

// Yanıt oluştur
var response = new DeviceResponse
{
    MessageType = MessageType.Handshake,
    ResponseCode = ResponseCode.Accept,
    ResponseTime = DateTime.Now.ToString("dd/MM/yy,HH:mm:ss")
};
``` 