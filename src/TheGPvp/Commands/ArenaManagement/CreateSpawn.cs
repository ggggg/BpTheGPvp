using BPEssentials.ExtensionMethods;
using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using BPCoreLib.Menus;
using BPCoreLib.Serializable;
using BrokeProtocol.Required;
using TheGPvp;
using TheGPvp.ArenaTypes;
using TheGPvp.BattleTypes;
using UnityEngine.UI;
using static TheGPvp.ArenaManager;

namespace TheGPvp.Commands
{
    internal class CreateSpawn : IScript
    {
        public CreateSpawn()
        {
            var cmds = new List<string> { "setspawn" };
            CommandHandler.RegisterCommand(cmds, new Action<ShPlayer, string>(OnCommandInvoke));
            Core.Instance.Logger.LogInfo("Registered " + nameof(CreateSpawn));
        }

        public void OnCommandInvoke(ShPlayer player, string arenaName)
        {
            var arena = Core.Instance.ArenaManager.Arenas.FirstOrDefault(x => x.ArenaSerializable.Name == arenaName);
            if (arena == null)
            {
                player.TS("arena_notfound", arenaName);
                return;
            }
            if (arena.ArenaSerializable.Type == TypeManager.AllType)
            {
                var labels = new List<LabelID>();
                var actions = new[] { new ActionLabel("Select", (cPlayer, id) => Create(cPlayer, id, arena)) };
                labels.AddRange(TypeManager.Instance.BattleTypes.Select(x => new LabelID(x.Key, x.Value.Name)));
                labels.Add(new LabelID("Any", TypeManager.AllType));
                player.SendOptionMenu("Select spawn type:", labels, actions);
                return;
            }
            Create(player, arena.ArenaSerializable.Type, arena);
        }

        private static void Create(ShPlayer player, string type, Arena arena)
        {
            var spawn = new BattleSpawn
            {
                X = player.GetPosition.x,
                Y = player.GetPosition.y,
                Z = player.GetPosition.z,
                Index = player.GetPlaceIndex,
                SpawnType = type,
                Rotation = player.GetRotation
            };
            if (Utils.IsSetSpawn(type))
            {
                player.SendInputMenu("enter_team_spawn_name", (x, input) =>
                {
                    spawn.TeamName = input;
                    AddSpawn(x, spawn, arena);
                });
            }
            else
            {
                AddSpawn(player, spawn, arena);
            }
        }

        private static void AddSpawn(ShPlayer player, BattleSpawn spawn, Arena arena)
        {
            arena.ArenaSerializable.Spawns.Add(spawn);
            player.TS("add_spawns", arena.ArenaSerializable.Name, arena.ArenaSerializable.Spawns.Count);
        }
    }
}