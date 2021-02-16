using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheGPvp.ExtensionMethods
{
    public static class SpawnExtensions
    {
        public static Vector3 FormatSpawn(this ArenaManager.BattleSpawn spawn)
        {
            return ((ArenaManager.Spawn)spawn).FormatSpawn();
        }

        public static Vector3 FormatSpawn(this ArenaManager.Spawn spawn)
        {
            return new Vector3(spawn.X, spawn.Y, spawn.Z);
        }

        public static Vector3 FormatSpawn(this ArenaManager.LobbySpawn spawn)
        {
            return ((ArenaManager.Spawn)spawn).FormatSpawn();
        }
    }
}