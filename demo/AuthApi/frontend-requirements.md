# Auth API Frontend Gereksinimleri / Auth API Frontend Requirements

## Türkçe / Turkish

### Genel Gereksinimler

#### Teknoloji Yığını
- **Frontend Framework**: React, Vue.js veya Angular
- **State Yönetimi**: Redux, Vuex veya Ngrx
- **HTTP İstemcisi**: Axios veya Fetch API
- **Form Yönetimi**: Formik, Vue Form veya Angular Reactive Forms
- **Validasyon**: Yup, Vuelidate veya Angular Validators
- **UI Kütüphanesi**: Material UI, Vuetify veya Angular Material
- **Dil Desteği**: i18next veya benzer çoklu dil desteği

#### Kimlik Doğrulama ve Yetkilendirme
- JWT token tabanlı kimlik doğrulama
- İki faktörlü kimlik doğrulama (2FA) desteği
- Yetkisiz erişim için yönlendirme mekanizması
- Oturum zaman aşımı yönetimi
- Token yenileme mekanizması

### Sayfa Gereksinimleri

#### Giriş Ekranı
- Kullanıcı adı/e-posta ve şifre alanları
- "Beni hatırla" seçeneği
- Şifremi unuttum bağlantısı
- Kayıt olma bağlantısı
- İki faktörlü kimlik doğrulama işlemi için kod giriş ekranı
- Form validasyonu
- Hata mesajları gösterimi
- Giriş işlemi sırasında yükleniyor göstergesi

#### Kayıt Ekranı
- Kullanıcı adı, e-posta, şifre ve şifre onayı alanları
- Ad-soyad alanı
- Kullanım şartları onay kutusu
- Form validasyonu (şifre güvenliği kontrolü dahil)
- Kayıt işlemi sırasında yükleniyor göstergesi
- Başarılı kayıt sonrası giriş sayfasına yönlendirme

#### Şifre Sıfırlama Ekranları
- E-posta giriş formu
- Sıfırlama kodu doğrulama ekranı
- Yeni şifre ve şifre onayı alanları
- Form validasyonu
- Başarılı şifre sıfırlama sonrası giriş sayfasına yönlendirme

#### Kullanıcı Profil Ekranı
- Kullanıcı bilgilerinin görüntülenmesi
- Profil düzenleme formu
- Profil resmi yükleme ve görüntüleme
- İki faktörlü kimlik doğrulama ayarları
- Şifre değiştirme bölümü

#### 2FA Ayarları Ekranı
- 2FA etkinleştirme/devre dışı bırakma seçeneği
- 2FA tipi seçimi (E-posta)
- Mevcut şifre doğrulama alanı
- Doğrulama kodu giriş alanı

### UX Gereksinimleri
- Tüm işlemler için yükleniyor göstergeleri
- Hata mesajları için toast veya banner bildirimleri
- Başarılı işlemler için bildirimler
- Duyarlı (responsive) tasarım (mobil, tablet, masaüstü)
- Karanlık/Aydınlık tema desteği
- Erişilebilirlik standartlarına uygunluk

### API Entegrasyonu
- Backend API ile iletişim için servis/hooks yapısı
- Token yönetimi için interceptor mekanizması
- Yeniden deneme ve hata işleme mekanizması
- Rate limiting durumunda bekleme/bildirim mekanizması
- 5xx hatalarında otomatik yeniden deneme
- API yanıt şemalarına uygun tip tanımları (TypeScript)

### Güvenlik Gereksinimleri
- Token'ların güvenli şekilde saklanması (HttpOnly cookies veya secure localStorage)
- XSS koruması
- CSRF koruması
- Hassas bilgilerin loglanmaması
- İnaktif durumlarda otomatik çıkış mekanizması
- API isteklerinde uygun güvenlik başlıklarının kullanımı

### Performans
- Sayfa yükleme süresi < 2 saniye
- Lazy loading ve code splitting
- Görüntü optimizasyonu
- Bileşenlerin memoizasyonu
- Gereksiz yeniden render'ların önlenmesi

### Test Gereksinimleri
- Birim testleri (%70+ kapsama)
- Entegrasyon testleri
- E2E testleri (kritik kullanıcı akışları için)
- Erişilebilirlik testleri

---

## English / İngilizce

### General Requirements

#### Technology Stack
- **Frontend Framework**: React, Vue.js, or Angular
- **State Management**: Redux, Vuex, or Ngrx
- **HTTP Client**: Axios or Fetch API
- **Form Management**: Formik, Vue Form, or Angular Reactive Forms
- **Validation**: Yup, Vuelidate, or Angular Validators
- **UI Library**: Material UI, Vuetify, or Angular Material
- **Language Support**: i18next or similar multi-language support

#### Authentication and Authorization
- JWT token-based authentication
- Two-factor authentication (2FA) support
- Redirection mechanism for unauthorized access
- Session timeout management
- Token refresh mechanism

### Page Requirements

#### Login Screen
- Username/email and password fields
- "Remember me" option
- Forgot password link
- Registration link
- Code entry screen for two-factor authentication
- Form validation
- Error message display
- Loading indicator during login process

#### Registration Screen
- Username, email, password, and confirm password fields
- Full name field
- Terms of use checkbox
- Form validation (including password security check)
- Loading indicator during registration process
- Redirection to login page after successful registration

#### Password Reset Screens
- Email entry form
- Reset code verification screen
- New password and password confirmation fields
- Form validation
- Redirection to login page after successful password reset

#### User Profile Screen
- Display of user information
- Profile editing form
- Profile picture upload and display
- Two-factor authentication settings
- Password change section

#### 2FA Settings Screen
- 2FA enable/disable option
- 2FA type selection (Email)
- Current password verification field
- Verification code entry field

### UX Requirements
- Loading indicators for all actions
- Toast or banner notifications for error messages
- Notifications for successful actions
- Responsive design (mobile, tablet, desktop)
- Dark/Light theme support
- Compliance with accessibility standards

### API Integration
- Service/hooks structure for communication with backend API
- Interceptor mechanism for token management
- Retry and error handling mechanism
- Waiting/notification mechanism for rate limiting
- Automatic retry on 5xx errors
- Type definitions matching API response schemas (TypeScript)

### Security Requirements
- Secure storage of tokens (HttpOnly cookies or secure localStorage)
- XSS protection
- CSRF protection
- No logging of sensitive information
- Automatic logout mechanism for inactive states
- Use of appropriate security headers in API requests

### Performance
- Page load time < 2 seconds
- Lazy loading and code splitting
- Image optimization
- Component memoization
- Prevention of unnecessary re-renders

### Test Requirements
- Unit tests (70%+ coverage)
- Integration tests
- E2E tests (for critical user flows)
- Accessibility tests 