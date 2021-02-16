using System;
using System.Diagnostics;
using BPEssentials.ExtensionMethods;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using System.Linq;
using TheGPvp.ExtensionMethods;
using System.Collections.Generic;

namespace TheGPvp.BattleTypes
{
    /// <summary>
    /// Team battle
    /// </summary>
    /// <typeparam name="T">The team type</typeparam>
    public abstract class TdmBattle<T> : Battle, ITdmBattle<T> where T : Team
    {
        public override int NumberOfTeams { get; }

        public override bool BattleWon => BattleStarted && AliveTeams.Length <= 1;

        public T[] Teams { get; }

        public T[] AliveTeams => Teams.Where(IsTeamAlive).ToArray();

        /// <summary>
        /// Check if a team is still part of the game
        /// </summary>
        /// <param name="team">The team to check</param>
        /// <returns> If a team is still part of the game</returns>
        protected virtual bool IsTeamAlive(T team)
        {
            return team != null && team.Players.Any(y => AlivePlayers.Contains(y));
        }

        protected TdmBattle(int numberOfTeams)
        {
            Debug.Assert(numberOfTeams > 1);
            NumberOfTeams = numberOfTeams;
            Teams = new T[numberOfTeams];
        }

        protected TdmBattle() : this(2)
        {
        }

        public override bool CheckForPlayers()
        {
            return Players.Count % NumberOfTeams == 0;
        }

        public override void OnWin(Action callback)
        {
            foreach (var player in new List<ShPlayer>(Players))
            {
                RemovePlayer(player);
            }
            callback.Invoke();
        }

        public override void RemovePlayer(ShPlayer player)
        {
            if (Players.Count <= 1 || !BattleStarted || BattleWon)
            {
                base.RemovePlayer(player);
                return;
            }

            var oldUsername = Players[0].username;
            base.RemovePlayer(player);
            if (BattleStarted)
            {
                GetPlayerTeam(player).Players.Remove(player);
                foreach (var cplayer in AlivePlayers.Where(cplayer => cplayer != player && cplayer != null))
                {
                    cplayer.TS("battleplayer_left", player.username.CleanerMessage(), AlivePlayers.Count);
                }
                return;
            }

            if (oldUsername == player.username)
            {
                Players[0].TS("tdm_leader", player.username.CleanerMessage(), Players.Count);
            }

            foreach (var cplayer in Players.Where(cplayer => cplayer != player && cplayer != null))
            {
                cplayer.TS("battleplayer_left", player.username.CleanerMessage(), Players.Count);
            }
        }

        public override bool TryStartBattle()
        {
            if (!base.TryStartBattle())
            {
                return false;
            }

            PopulateTeams();

            return true;
        }

        /// <summary>
        /// Split all the players into a team and add each team to the Teams array.
        /// </summary>
        /// <see cref="Teams"/>
        public abstract void PopulateTeams();

        public override void SpawnPlayers(IList<ArenaManager.BattleSpawn> spawns)
        {
            // do not call base.SpawnPlayers because this will try to spawn each player in a different spawn
            for (var i = 0; i < NumberOfTeams; i++)
            {
                Teams[i].Spawn = spawns[i];
                foreach (var player in Teams[i].Players)
                {
                    player.GetExtendedPlayerPvp().TpToSpawn(Teams[i].Spawn);
                }
            }
        }

        public T GetPlayerTeam(ShPlayer player)
        {
            return Teams.FirstOrDefault(x => x.Players.Contains(player));
        }
    }

    public class Team
    {
        public string Name { get; set; }

        public ArenaManager.BattleSpawn Spawn { get; set; }

        public List<ShPlayer> Players { get; } = new List<ShPlayer>();

        public Team(string name)
        {
            Name = name;
        }

        public Team(string name, IEnumerable<ShPlayer> players) : this(name)
        {
            Players = players.ToList();
        }
    }
}