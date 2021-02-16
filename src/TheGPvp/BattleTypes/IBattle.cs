using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrokeProtocol.Entities;

namespace TheGPvp.BattleTypes
{
    public interface IBattle
    {
        bool CheckForPlayers();

        bool BattleStarted { get; set; }

        List<ShPlayer> Players { get; set; }

        List<ShPlayer> AlivePlayers { get; set; }

        int NumberOfTeams { get; }

        bool BattleWon { get; }

        bool TryStartBattle();

        bool TryAddPlayerToBattle(ShPlayer player);

        void OnWin(Action callback);

        void RemovePlayer(ShPlayer player);

        void DisconnectPlayer(ShPlayer player);

        void OnPlayerDeath(ShPlayer player, ShPlayer attacker);

        void SpawnPlayers(IList<ArenaManager.BattleSpawn> spawns);
    }
}