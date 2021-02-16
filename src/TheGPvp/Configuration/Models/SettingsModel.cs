using System.Collections.Generic;

namespace TheGPvp.Configuration.Models.SettingsModel
{
    public class Settings
    {
        public General General { get; set; }
    }

    public class General
    {
        public string Version { get; set; }
        public int MaxLobbyTime { get; set; }
        public int LottingTime { get; set; }
    }
}