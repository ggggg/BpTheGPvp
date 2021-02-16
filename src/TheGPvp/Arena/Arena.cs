using System;
using BPEssentials.ExtensionMethods;
using BrokeProtocol.Entities;
using System.Collections.Generic;
using System.Linq;
using TheGPvp.BattleTypes;
using TheGPvp.ExtensionMethods;
using static TheGPvp.ArenaManager;

namespace TheGPvp.ArenaTypes
{
    public class Arena
    {
        private void Reset()
        {
            ResetGlobal();
            Battle = null;
        }

        private void ResetGlobal()
        {
            if (Core.Instance.ArenaManager.GlobalPvp[ArenaType] == this)
            {
                Core.Instance.ArenaManager.GlobalPvp[ArenaType] = null;
            }
        }

        private string ArenaType => InUse && ArenaSerilizable.Type == TypeManager.AllType
            ? Utils.FormatBattleType(Battle)
            : ArenaSerilizable.Type;

        public Arena(ArenaSerilizable arenaSerilizable)
        {
            ArenaSerilizable = arenaSerilizable;
        }

        public ArenaSerilizable ArenaSerilizable { get; set; }

        public bool InUse => Battle != null;

        public IBattle Battle { get; set; }

        public List<BattleSpawn> GetValidSpawns(string type)
        {
            var setSpawn = Utils.IsSetSpawn(type);
            return ArenaSerilizable.Type != TypeManager.AllType
                ? ArenaSerilizable.Spawns
                : ArenaSerilizable.Spawns.Where(x =>
                    ((!setSpawn && x.SpawnType == TypeManager.AllType) || x.SpawnType == type)).ToList();
        }

        public List<BattleSpawn> GetValidSpawns()
        {
            return GetValidSpawns(ArenaType);
        }

        public void Save()
        {
            Core.Instance.ArenaManager.ArenaCollection.Upsert(ArenaSerilizable);
        }

        public void Start<T>(ShPlayer player, T battle, bool global = true) where T : IBattle
        {
            Battle = battle;
            battle.BattleStarted = false;
            TryAddPlayerToBattle(player);
            if (global)
            {
                Core.Instance.ArenaManager.GlobalPvp[ArenaType] = this;
            }

            LobbyTimer(battle);
        }

        private void LobbyTimer(IBattle battle)
        {
            Core.Instance.StartMethodTimer(Core.Instance.Settings.General.MaxLobbyTime, () =>
            {
                if (Battle == null || Battle.BattleStarted || !ReferenceEquals(battle, Battle)) return;
                if (Battle.CheckForPlayers())
                {
                    Battle.Players[0].TS("battle_leader_start");
                    LobbyTimer(battle);
                    return;
                }
                foreach (var cplayer in battle.Players)
                {
                    cplayer.TS("not_enoughjoined");
                    RemovePlayerFromBattle(cplayer);
                }
            });
        }

        public bool TryAddPlayerToBattle(ShPlayer player)
        {
            if (Battle.NumberOfTeams > GetValidSpawns().Count || !Battle.TryAddPlayerToBattle(player))
            {
                return false;
            }
            player.GetExtendedPlayerPvp().TpToSpawnSave(ArenaSerilizable.Lobby);
            return true;
        }

        public bool TryStartBattle()
        {
            if (!Battle.TryStartBattle())
            {
                return false;
            }

            Battle.SpawnPlayers(GetValidSpawns().Shuffle());
            if (Core.Instance.Kits.ContainsKey(ArenaSerilizable.Name))
            {
                GiveKitToAllPlayers();
            }

            ResetGlobal();
            return true;
        }

        private void GiveKitToAllPlayers()
        {
            var battleType = Utils.FormatBattleType(Battle);
            if (!Core.Instance.Kits[ArenaSerilizable.Name].ContainsKey(battleType)) return;
            var kits = Core.Instance.Kits[ArenaSerilizable.Name][battleType];
            foreach (var player in Battle.Players)
            {
                player.GetExtendedPlayerPvp()?.GiveKitSave(kits);
            }
        }

        public void Win()
        {
            Battle.OnWin(() =>
            {
                if (Battle.AlivePlayers.Count == 0)
                {
                    Reset();
                    return;
                }
                Battle = null;
            });
        }

        public void RemovePlayerFromBattle(ShPlayer player)
        {
            Battle.RemovePlayer(player);
            if (Battle.BattleStarted)
            {
                if (Battle.BattleWon)
                {
                    Win();
                    return;
                }
            }
            if (Battle.Players.Count <= 0)
            {
                Reset();
            }
        }

        public void OnPlayerDeath(ShPlayer player, ShPlayer attacker)
        {
            if (!Battle.BattleStarted)
            {
                RemovePlayerFromBattle(player);
                return;
            }
            Battle.OnPlayerDeath(player, attacker);
            if (Battle.BattleWon)
            {
                Win();
            }
        }

        public void DisconnectPlayer(ShPlayer player)
        {
            Battle.DisconnectPlayer(player);
            if (Battle.BattleStarted && Battle.BattleWon)
            {
                Win();
                return;
            }
            if (Battle.Players.Count <= 0)
            {
                Reset();
            }
        }
    }
}