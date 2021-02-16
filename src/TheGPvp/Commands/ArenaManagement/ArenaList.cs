using BPEssentials.ExtensionMethods;
using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static TheGPvp.ArenaManager;

namespace TheGPvp.Commands
{
    internal class ArenaList : IScript
    {
        public ArenaList()
        {
            List<string> cmds = new List<string> { "arenalist" };
            CommandHandler.RegisterCommand(cmds, new Action<ShPlayer>(OnCommandInvoke));
            Core.Instance.Logger.LogInfo("Registered " + nameof(ArenaList));
        }

        public void OnCommandInvoke(ShPlayer player)
        {
            var list = Core.Instance.ArenaManager.Arenas.Aggregate("", (current, arena) => current + (arena.ArenaSerializable.Name + " | "));
            player.TS("arena_list", list);
        }
    }
}