using BPEssentials.ExtensionMethods;
using TheGPvp;
using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using static TheGPvp.ArenaManager;
using System.Linq;
using BPCoreLib.Menus;
using TheGPvp.ArenaTypes;
using BrokeProtocol.Required;
using TheGPvp.BattleTypes;

namespace TheGPvp.Commands
{
    public class CreateArena : IScript
    {
        public CreateArena()
        {
            List<string> cmds = new List<string> { "arenasetup", "createarena" };
            CommandHandler.RegisterCommand(cmds, new Action<ShPlayer, string>(OnCommandInvoke));
            Core.Instance.Logger.LogInfo("Registered " + nameof(CreateArena));
        }

        public void OnCommandInvoke(ShPlayer player, string name)
        {
            if (Core.Instance.ArenaManager.Arenas.FirstOrDefault(x => x.ArenaSerializable.Name == name) != null)
            {
                player.TS("arena_nametaken", name);
                return;
            }

            var i = 0;
            Core.Instance.Logger.LogInfo(++i + "");
            var actions = new[]{new ActionLabel("Select", (cPlayer, id) =>
            {
                if (Core.Instance.ArenaManager.Arenas.FirstOrDefault(x => x.ArenaSerializable.Name == name) != null)
                {
                    player.TS("arena_nametaken", name);
                    return;
                }

                var arena = new Arena(new ArenaSerializable()) {ArenaSerializable = {Name = name, Type = id}};
                Core.Instance.ArenaManager.Arenas.Add(arena);
                Core.Instance.ArenaManager.ArenaCollection.Insert(arena.ArenaSerializable);
                cPlayer.TS("arena_created", name);
            })};
            Core.Instance.Logger.LogInfo(++i + "");
            var labels = TypeManager.Instance.BattleTypes.Select(x => new LabelID(x.Key, x.Value.Name)).ToList();
            Core.Instance.Logger.LogInfo(++i + "");
            labels.Add(new LabelID("All", TypeManager.AllType));
            Core.Instance.Logger.LogInfo(++i + "");
            player.SendOptionMenu("Select arena type:", labels, actions);
        }
    }
}