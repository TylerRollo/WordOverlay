using System;
using System.IO;
using System.Text.Json;
using WordOverlay.Models;

namespace WordOverlay.Services
{
    public class ConfigService
    {
        private readonly string _configPath;

        public ConfigService()
        {
            string appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WordOverlay");

            Directory.CreateDirectory(appDataDir);
            _configPath = Path.Combine(appDataDir, "config.json");
        }

        public OverlayConfig Load()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    string json = File.ReadAllText(_configPath);
                    var config = JsonSerializer.Deserialize<OverlayConfig>(json);
                    if (config != null)
                        return config;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load config: {ex.Message}");
            }

            return new OverlayConfig();
        }

        public void Save(OverlayConfig config)
        {
            try
            {
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save config: {ex.Message}");
            }
        }
    }
}
