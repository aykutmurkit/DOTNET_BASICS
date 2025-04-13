using System;

namespace Data.Seeding
{
    /// <summary>
    /// Seeder sınıfları için uzantı metodları
    /// </summary>
    public static class SeederExtensions
    {
        /// <summary>
        /// Seeder sınıfı için çalışma sırasını döndürür
        /// </summary>
        public static int GetOrder<T>(this T seeder) where T : ISeeder
        {
            return seeder.GetType().Name switch
            {
                nameof(StationSeeder) => (int)SeederOrder.Stations,
                nameof(PlatformSeeder) => (int)SeederOrder.Platforms,
                nameof(DeviceSeeder) => (int)SeederOrder.Devices,
                nameof(DeviceSettingsSeeder) => (int)SeederOrder.DeviceSettings,
                nameof(DeviceStatusSeeder) => (int)SeederOrder.DeviceStatuses,
                nameof(PredictionSeeder) => (int)SeederOrder.Predictions,
                nameof(FullScreenMessageSeeder) => (int)SeederOrder.FullScreenMessages,
                nameof(ScrollingScreenMessageSeeder) => (int)SeederOrder.ScrollingScreenMessages,
                nameof(BitmapScreenMessageSeeder) => (int)SeederOrder.BitmapScreenMessages,
                nameof(PeriodicMessageSeeder) => (int)SeederOrder.PeriodicMessages,
                nameof(ScheduleRuleSeeder) => (int)SeederOrder.ScheduleRules,
                _ => throw new NotImplementedException($"{seeder.GetType().Name} için sıralama tanımlanmamış.")
            };
        }
    }
} 