using BPEssentials.ExtensionMethods;
using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheGPvp.ArenaManager;

namespace TheGPvp.Commands
{
    public class ArenaDelete : IScript
    {
        public ArenaDelete()
        {
            List<string> cmds = new List<string> { "deletearena" };
            CommandHandler.RegisterCommand(cmds, new Action<ShPlayer, string>(OnCommandInvoke));
            Core.Instance.Logger.LogInfo("Registered " + nameof(ArenaDelete));
        }

        public void OnCommandInvoke(ShPlayer player, string arenaName)
        {
            var arena = Core.Instance.ArenaManager.Arenas.FirstOrDefault(x => x.ArenaSerializable.Name == arenaName);
            if (arena == null)
            {
                player.TS("arena_notfound", arenaName);
                return;
            }
            Core.Instance.ArenaManager.Arenas.Remove(arena);
            Core.Instance.ArenaManager.ArenaCollection.Delete(arena.ArenaSerializable.Name);
            player.TS("arena_deleted", arenaName);
        }
    }
}