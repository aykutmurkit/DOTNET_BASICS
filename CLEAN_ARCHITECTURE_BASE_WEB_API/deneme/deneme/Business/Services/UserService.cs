using Business.Interfaces;
using Core.Security;
using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System;
using System.Drawing.Imaging;

namespace Business.Services
{
    /// <summary>
    /// Kullanıcı servisi implementasyonu
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITwoFactorService _twoFactorService;
        private readonly AppDbContext _context;

        public UserService(IUserRepository userRepository, ITwoFactorService twoFactorService, AppDbContext context)
        {
            _userRepository = userRepository;
            _twoFactorService = twoFactorService;
            _context = context;
        }

        /// <summary>
        /// Tüm kullanıcıları getirir
        /// </summary>
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(u => MapToUserDto(u, includeProfilePicture: false)).ToList();
        }

        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            return MapToUserDto(user, includeProfilePicture: true);
        }

        /// <summary>
        /// Kullanıcı oluşturur
        /// </summary>
        public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
        {
            // Kullanıcı adı ve e-posta kontrolü
            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                throw new Exception("Bu kullanıcı adı zaten kullanılıyor.");
            }

            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new Exception("Bu e-posta adresi zaten kullanılıyor.");
            }

            // Rol kontrolü
            var role = await _context.UserRoles.FindAsync(request.RoleId);
            if (role == null)
            {
                throw new Exception("Geçersiz rol ID'si.");
            }

            // Şifre hashleme
            string salt = PasswordHelper.CreateSalt();
            string passwordHash = PasswordHelper.HashPassword(request.Password, salt);

            // Yeni kullanıcı oluşturma
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                RoleId = request.RoleId,
                CreatedDate = DateTime.UtcNow,
                ProfilePicture = GetDefaultProfilePicture()
            };

            await _userRepository.AddUserAsync(user);
            return MapToUserDto(user, includeProfilePicture: true);
        }

        /// <summary>
        /// Kullanıcı günceller
        /// </summary>
        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            // Kullanıcı adı kontrolü
            if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
            {
                if (await _userRepository.UsernameExistsAsync(request.Username))
                {
                    throw new Exception("Bu kullanıcı adı zaten kullanılıyor.");
                }
                user.Username = request.Username;
            }

            // E-posta kontrolü
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    throw new Exception("Bu e-posta adresi zaten kullanılıyor.");
                }
                user.Email = request.Email;
            }

            // Rol güncelleme
            if (request.RoleId.HasValue)
            {
                var role = await _context.UserRoles.FindAsync(request.RoleId);
                if (role == null)
                {
                    throw new Exception("Geçersiz rol ID'si.");
                }
                user.RoleId = request.RoleId.Value;
            }

            await _userRepository.UpdateUserAsync(user);
            return MapToUserDto(user, includeProfilePicture: true);
        }

        /// <summary>
        /// Kullanıcı siler
        /// </summary>
        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            await _userRepository.DeleteUserAsync(id);
        }

        /// <summary>
        /// Profil fotoğrafı yükler
        /// </summary>
        public async Task UploadProfilePictureAsync(int userId, UploadProfilePictureRequest request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            // Dosya tipini kontrol et (sadece resim dosyaları)
            if (!request.ProfilePicture.ContentType.StartsWith("image/"))
            {
                throw new Exception("Sadece resim dosyaları yüklenebilir.");
            }

            // Dosyayı memory'ye yükle
            using var memoryStream = new MemoryStream();
            await request.ProfilePicture.CopyToAsync(memoryStream);
            
            // Resim boyutlarını kontrol et
            using var image = Image.FromStream(memoryStream);
            
            // Kare resim kontrolü (yatay ve dikey boyut aynı olmalı)
            if (image.Width != image.Height)
            {
                throw new Exception("Profil fotoğrafı kare formatta olmalıdır (örn. 200x200).");
            }
            
            // Maksimum boyut kontrolü
            if (image.Width > 1000 || image.Height > 1000)
            {
                throw new Exception("Profil fotoğrafı maksimum 1000x1000 piksel boyutunda olmalıdır.");
            }
            
            // Dosyayı byte array'e dönüştür ve kaydet
            memoryStream.Position = 0;
            user.ProfilePicture = memoryStream.ToArray();

            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Profil fotoğrafını getirir
        /// </summary>
        public async Task<byte[]> GetProfilePictureAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            return user.ProfilePicture ?? GetDefaultProfilePicture();
        }

        /// <summary>
        /// Varsayılan profil fotoğrafı oluşturur (kırmızı kare)
        /// </summary>
        private byte[] GetDefaultProfilePicture()
        {
            // 200x200 boyutunda kare oluştur (kare olma gereksinimini karşılamak için)
            using var bitmap = new Bitmap(200, 200);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Red);

            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// User nesnesini UserDto'ya dönüştürür
        /// </summary>
        /// <param name="user">Kullanıcı nesnesi</param>
        /// <param name="includeProfilePicture">Profil fotoğrafını dahil et</param>
        private UserDto MapToUserDto(User user, bool includeProfilePicture = false)
        {
            var profilePictureInfo = new ProfilePictureInfo
            {
                HasProfilePicture = user.ProfilePicture != null
            };
            
            // Sadece tek kullanıcı görüntülenirken profil fotoğrafını dahil et
            if (includeProfilePicture && user.ProfilePicture != null)
            {
                profilePictureInfo.Url = $"/api/Users/{user.Id}/profile-picture";
                profilePictureInfo.Picture = Convert.ToBase64String(user.ProfilePicture);
            }
            
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = new RoleInfo 
                {
                    Id = user.RoleId,
                    Name = user.Role?.Name ?? "Unknown"
                },
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                TwoFactor = new TwoFactorInfo
                {
                    Enabled = user.TwoFactorEnabled,
                    Required = _twoFactorService.IsTwoFactorRequired()
                },
                ProfilePicture = profilePictureInfo
            };
        }
    }
} 