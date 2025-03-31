# Deneme API Dokümantasyonu

Bu klasör, Deneme API projesi ile ilgili teknik dokümantasyonu içerir.

## İçindekiler

- [Mimari Dokümantasyonu](#mimari-dokümantasyonu)
- [Loglama Sistemi](#loglama-sistemi)
- [API Kullanımı](#api-kullanımı)
- [Veritabanı Şeması](#veritabanı-şeması)

## Mimari Dokümantasyonu

Deneme API, modern .NET 8 teknolojilerini kullanan bir N-Tier (Çok Katmanlı) mimari üzerine inşa edilmiştir. Bu yapı, uygulamayı mantıksal olarak birbirinden ayrılmış katmanlara bölerek, daha modüler, test edilebilir ve bakımı kolay hale getirir.

Detaylı bilgi için [N-Tier Mimari Dokümantasyonu](N-Tier-Architecture.md) sayfasına bakabilirsiniz.

## Loglama Sistemi

Deneme API, MongoDB tabanlı kapsamlı bir loglama sistemi kullanır. Bu sistem, gelen istekleri, yanıtları ve sistem olaylarını kaydeder ve analiz amacıyla saklar.

Detaylı bilgi için [Loglama Sistemi Dokümantasyonu](logging-system.md) sayfasına bakabilirsiniz.

## API Kullanımı

Deneme API, REST standartlarını takip eden bir API sunar. Tüm istekler JSON formatında veri alır ve döndürür.

API dokümantasyonu, OpenAPI (Swagger) kullanılarak otomatik olarak oluşturulur ve şu adresten erişilebilir: `https://{base-url}/swagger`

## Veritabanı Şeması

Deneme API, birincil veritabanı olarak SQL Server, loglama için ise MongoDB kullanır. Veritabanı şeması, Entity Framework Core Code-First yaklaşımı ile oluşturulmuştur.

Veritabanı yapılandırması ve migration işlemleri hakkında detaylı bilgi için [Veritabanı Şeması Dokümantasyonu] sayfasına bakabilirsiniz (henüz oluşturulmamıştır).

## Katkıda Bulunma

Projeye katkıda bulunmak için aşağıdaki adımları izleyebilirsiniz:

1. Bu repo'yu fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylı bilgi için `LICENSE` dosyasına bakabilirsiniz. 