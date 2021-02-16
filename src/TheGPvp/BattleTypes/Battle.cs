using System;
using BPEssentials.ExtensionMethods;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using System.Collections.Generic;
using System.Linq;
using TheGPvp.ExtensionMethods;

namespace TheGPvp.BattleTypes
{
    /// <summary>
    /// All non abstract subclasses of this class automatically get loaded at server start, extend this class to create a battle
    /// </summary>
    public abstract class Battle : IBattle
    {
        /// <summary>
        /// Start a new battle.
        /// </summary>
        protected Battle()
        {
            BattleStarted = false;
        }

        /// <summary>
        /// Check if there are enough players to start a battle.
        /// </summary>
        /// <returns>
        /// Whether there are enough players to start a battle.
        /// </returns>
        public abstract bool CheckForPlayers();

        /// <summary>
        /// Whether if the battle is finished
        /// </summary>
        public virtual bool BattleWon => AlivePlayers.Count <= 1;

        /// <summary>
        /// If false, it means the battle hasn't started yet but is in lobby.
        /// </summary>
        public bool BattleStarted { get; set; }

        /// <summary>
        /// The number of spawns this battle type needs.
        /// </summary>
        public virtual int NumberOfTeams => Players.Count;

        /// <summary>
        /// All players who joined the battle (might be already dead)
        /// </summary>
        public List<ShPlayer> Players { get; set; } = new List<ShPlayer>();

        /// <summary>
        /// All alive players
        /// </summary>
        public List<ShPlayer> AlivePlayers { get; set; } = new List<ShPlayer>();

        /// <summary>
        /// Check if the battle is ok to start (number of players and if it is already started), then heal all the players and disable god mode
        /// </summary>
        /// <returns> if the battle is ok to start (number of players and if it is already started) </returns>
        public virtual bool TryStartBattle()
        {
            if (BattleStarted || !CheckForPlayers())
            {
                return false;
            }
            AlivePlayers = new List<ShPlayer>(Players);
            AlivePlayers.ForEach(x =>
            {
                x.svPlayer.SvHeal(x.maxStat);
                x.svPlayer.SvClearInjuries();
                x.svPlayer.godMode = false;
            });
            BattleStarted = true;
            return true;
        }

        /// <summary>
        /// Add a player to battle.
        /// </summary>
        /// <param name="player">The player to add</param>
        /// <returns>If battle started already</returns>
        public bool TryAddPlayerToBattle(ShPlayer player)
        {
            if (BattleStarted)
            {
                return false;
            }
            if (!player.GetExtendedPlayerPvp().CrimeLoop)
            {
                player.GetExtendedPlayerPvp().AddCrimes();
            }
            else
            {
                player.GetExtendedPlayerPvp().ClearCrimes();
                player.AddCrime(CrimeIndex.Bombing, null);
                player.svPlayer.Send(SvSendType.Self, Channel.Reliable, ClPacket.AddCrime, CrimeIndex.Bombing, 0);
            }
            player.svPlayer.godMode = true;
            foreach (var cplayer in Players)
            {
                cplayer.TS("battleplayer_joined", player.username.CleanerMessage(), Players.Count);
            }
            Players.Add(player);
            return true;
        }

        /// <summary>
        /// When a player disconnects in battle.
        /// </summary>
        /// <param name="player">The player that diconnected</param>
        public virtual void DisconnectPlayer(ShPlayer player)
        {
            RemovePlayer(player);
            var list = BattleStarted ? AlivePlayers : Players;
            foreach (var cplayer in list.Where(cplayer => cplayer != player && cplayer != null))
            {
                cplayer.TS("battleplayer_left", player.username.CleanerMessage(), list.Count);
            }
        }

        /// <summary>
        /// Remove a player from battle.
        /// </summary>
        /// <param name="player">The player to remove</param>
        public virtual void RemovePlayer(ShPlayer player)
        {
            if (BattleStarted)
                AlivePlayers.Remove(player);
            else
                player.svPlayer.godMode = false;
            Players.Remove(player);
            //clear the crimes
            player.ClearCrimes();
            player.svPlayer.Send(SvSendType.Self, Channel.Reliable, ClPacket.ClearCrimes, player.ID);
            player.GetExtendedPlayerPvp().TpPlayerToLastLocation();
            player.GetExtendedPlayerPvp().GiveItemsBack();
        }

        public virtual void OnWin(Action callback)
        {
            foreach (var player in AlivePlayers)
            {
                player.TS("lotting_time", Core.Instance.Settings.General.LottingTime);
            }
            Core.Instance.StartMethodTimer(Core.Instance.Settings.General.LottingTime, () =>
            {
                RemovePlayer(AlivePlayers[0]);
                callback.Invoke();
            });
        }

        public virtual void OnPlayerDeath(ShPlayer player, ShPlayer attacker)
        {
            AlivePlayers.Remove(player);
            player.GetExtendedPlayerPvp().GiveItemsBack();
            if (BattleWon)
            {
                return;
            }
            foreach (var praticepent in AlivePlayers)
            {
                praticepent.TS("pvpplayer_died", player.username, AlivePlayers.Count);
            }
        }

        /// <summary>
        /// Spawn all the players
        /// </summary>
        /// <param name="spawns">A list of valid spawn points, spawns.Count >= Players.Count </param>
        public virtual void SpawnPlayers(IList<ArenaManager.BattleSpawn> spawns)
        {
            for (var i = 0; i < NumberOfTeams; i++)
            {
                AlivePlayers[i].GetExtendedPlayerPvp().TpToSpawn(spawns[i]);
            }
        }
    }
}