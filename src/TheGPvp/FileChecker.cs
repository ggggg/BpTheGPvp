using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TheGPvp
{
    public static class FileChecker
    {
        public static Dictionary<string, string> RequiredDirectories { get; } = new Dictionary<string, string>
        {
            {"TheGPvp", Paths.Folder}
        };

        public static Dictionary<string, string> RequiredFiles { get; } = new Dictionary<string, string>
        {
            {"settings.json", Core.Instance.Paths.SettingsFile},
            {"localization.json", Core.Instance.Paths.LocalizationFile}
        };

        public static HttpClient Client { get; } = new HttpClient();

        public static async Task CheckFiles()
        {
            foreach (var directory in RequiredDirectories.Where(directory => !Directory.Exists(directory.Value)))
            {
                Core.Instance.Logger.LogWarning($"{directory.Key} directory was not found; creating");
                Directory.CreateDirectory(directory.Value);
                Core.Instance.Logger.LogInfo($"{directory.Key} directory was created.");
            }

            foreach (var file in RequiredFiles.Where(file => !File.Exists(file.Value)))
            {
                Core.Instance.Logger.LogError($"{file.Key} was not found; downloading.");
                var content = await Client.GetStringAsync($"https://raw.githubusercontent.com/ggggg/file-download/master/TheGPvp/{file.Key}");
                File.WriteAllText(file.Value, content);
                Core.Instance.Logger.LogInfo($"{file.Key} was downloaded.");
            }
        }
    }
}