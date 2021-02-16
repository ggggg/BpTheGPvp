using System.IO;

namespace TheGPvp
{
    public class Paths
    {
        public static string Folder { get; } = "TheGPvp";

        public string SettingsFile { get; } = Path.Combine(Folder, "settings.json");

        public string KitsFile { get; } = Path.Combine(Folder, "kits.json");

        public string LocalizationFile { get; } = Path.Combine(Folder, "localization.json");
    }
}