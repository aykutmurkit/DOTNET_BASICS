using Data.Context;

namespace Data.Seeding
{
    /// <summary>
    /// Seed data işlemleri için arayüz
    /// </summary>
    public interface ISeeder
    {
        /// <summary>
        /// Seed etme sirasini belirler. Dusuk sayilar once calisir.
        /// </summary>
        /// <remarks>
        /// Bu property SeederExtensions.GetOrder() metodu ile otomatik olarak alınabilir.
        /// </remarks>
        int Order => this.GetOrder();

        /// <summary>
        /// Seed islemini gerceklestirir
        /// </summary>
        /// <param name="context">Veritabani baglanti context'i</param>
        Task SeedAsync(AppDbContext context);
    }
} 