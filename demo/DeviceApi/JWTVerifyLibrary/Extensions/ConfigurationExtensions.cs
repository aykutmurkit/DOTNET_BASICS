using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace JWTVerifyLibrary.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration LoadJwtVerifyLibrarySettings(this IConfiguration configuration)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var libraryDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            // Try multiple possible locations for the settings file
            var possiblePaths = new[]
            {
                Path.Combine(currentDirectory, "JWTVerifyLibrarySettings.json"),
                Path.Combine(libraryDirectory, "JWTVerifyLibrarySettings.json"),
                Path.Combine(currentDirectory, "Config", "JWTVerifyLibrarySettings.json")
            };

            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration);

            bool fileFound = false;
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    configBuilder.AddJsonFile(path, optional: false);
                    fileFound = true;
                    break;
                }
            }

            // If file wasn't found in any of the predefined locations, 
            // try to create it in the current directory with default values
            if (!fileFound)
            {
                var defaultSettingsPath = Path.Combine(currentDirectory, "JWTVerifyLibrarySettings.json");
                var defaultSettings = @"{
  ""JwtSettings"": {
    ""Secret"": ""VerySecureSecretKey12345678901234567890"",
    ""Issuer"": ""DenemeApi"",
    ""Audience"": ""DenemeApiClient"",
    ""AccessTokenExpirationInMinutes"": 60,
    ""RefreshTokenExpirationInDays"": 7
  }
}";
                File.WriteAllText(defaultSettingsPath, defaultSettings);
                configBuilder.AddJsonFile(defaultSettingsPath, optional: false);
            }

            return configBuilder.Build();
        }
    }
} 