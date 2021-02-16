using BPCoreLib.Interfaces;
using BPCoreLib.Util;
using TheGPvp.Configuration.Models.SettingsModel;
using BrokeProtocol.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BrokeProtocol.Managers;
using TheGPvp.ExtendedPlayer;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using TheGPvp.Configuration.Models;

namespace TheGPvp
{
    public class Core : Plugin
    {
        public static Core Instance { get; internal set; }

        public static string Version { get; } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public BPCoreLib.PlayerFactory.ExtendedPlayerFactory<PlayerItem> PlayerHandler { get; internal set; } = new ExtendedPlayerFactory();

        public BPCoreLib.Interfaces.ILogger Logger { get; } = new BPCoreLib.Util.Logger();

        public Paths Paths { get; } = new Paths();

        private IReader<Settings> SettingsReader { get; } = new Reader<Settings>();

        // TODO this is pretty ugly
        private IReader<Dictionary<string, Dictionary<string, List<KitModel>>>> KitsReader { get; } = new Reader<Dictionary<string, Dictionary<string, List<KitModel>>>>();

        public Settings Settings => SettingsReader.Parsed;

        public Dictionary<string, Dictionary<string, List<KitModel>>> Kits => File.Exists(Paths.KitsFile) ? KitsReader.Parsed : new Dictionary<string, Dictionary<string, List<KitModel>>>();

        public I18n I18n { get; set; }

        public SvManager SvManager { get; set; }

        public ArenaManager ArenaManager { get; set; }

        public RankManager RankManager { get; set; }

        public Core()
        {
            Instance = this;
            Info = new PluginInfo("TheGPvp", "TheGPvp")
            {
                Description = "PointLife custom pvp Content"
            };

            EventsHandler.Add("TheGPvp:reload", new Action(OnReloadRequestAsync));

            ArenaManager = new ArenaManager();
            RankManager = new RankManager();
            OnReloadRequestAsync();
            SetCutsomData();

            EventsHandler.Add("TheGPvp:version", new Action<string>(OnVersionRequest));
            Logger.LogInfo($"BP PvP {(IsDevelopmentBuild() ? "[DEVELOPMENT-BUILD] " : "")}v{Version} loaded in successfully!");
        }

        private void SetCutsomData()
        {
            CustomData.AddOrUpdate("version", Version);
            CustomData.AddOrUpdate("devbuild", IsDevelopmentBuild());
        }

        public static bool IsDevelopmentBuild()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        private static IEnumerator MethodTimer(float time, Action action)
        {
            yield return new WaitForSecondsRealtime(time);
            action.Invoke();
        }

        public void StartMethodTimer(float time, Action action)
        {
            Core.Instance.SvManager.StartCoroutine(MethodTimer(time, action));
        }

        public void SetupI18n()
        {
            I18n = new I18n();
            I18n.ParseLocalization(Paths.LocalizationFile);
            foreach (var lang in I18n.Reader.Parsed)
            {
                foreach (var data in lang.Value.Where(data => !BPEssentials.Core.Instance.I18n.Reader.Parsed[lang.Key].ContainsKey(data.Key)))
                {
                    BPEssentials.Core.Instance.I18n.Reader.Parsed[lang.Key][data.Key] = data.Value;
                }
            }
            Logger.LogInfo("I18n injected!");
        }

        public void SetConfigurationFilePaths()
        {
            SettingsReader.Path = Paths.SettingsFile;
            if (File.Exists(Paths.KitsFile))
                KitsReader.Path = Paths.KitsFile;
        }

        public void ReadConfigurationFiles()
        {
            SettingsReader.ReadAndParse();
            if (File.Exists(Paths.KitsFile))
                KitsReader.ReadAndParse();
        }

        public async void OnReloadRequestAsync()
        {
            SetConfigurationFilePaths();
            await FileChecker.CheckFiles();
            ReadConfigurationFiles();
            SetupI18n();
        }

        public void OnVersionRequest(string callback)
        {
            if (!callback.StartsWith(Core.Instance.Info.GroupNamespace + ":"))
            {
                return;
            }
            EventsHandler.Exec(callback, Version, IsDevelopmentBuild());
        }
    }
}