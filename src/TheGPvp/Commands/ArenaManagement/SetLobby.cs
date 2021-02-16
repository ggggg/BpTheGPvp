using BPEssentials.ExtensionMethods;
using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using BPCoreLib.Serializable;
using TheGPvp;
using static TheGPvp.ArenaManager;

namespace TheGPvp.Commands
{
    internal class SetLobby : IScript
    {
        public SetLobby()
        {
            List<string> cmds = new List<string> { "setlobby" };
            CommandHandler.RegisterCommand(cmds, new Action<ShPlayer, string>(OnCommandInvoke));
            Core.Instance.Logger.LogInfo("Registered " + nameof(SetLobby));
        }

        public void OnCommandInvoke(ShPlayer player, string arenaName)
        {
            var arena = Core.Instance.ArenaManager.Arenas.FirstOrDefault(x => x.ArenaSerializable.Name == arenaName);
            if (arena == null)
            {
                player.TS("arena_notfound", arenaName);
                return;
            }
            var spawn = new LobbySpawn
            {
                X = player.GetPosition.x,
                Y = player.GetPosition.y,
                Z = player.GetPosition.z,
                Index = player.GetPlaceIndex,
                Rotation = player.GetRotation
            };
            arena.ArenaSerializable.Lobby = spawn;
            player.TS("add_lobby", arenaName);
        }
    }
}