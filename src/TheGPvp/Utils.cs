using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheGPvp.BattleTypes;

namespace TheGPvp
{
    public static class Utils
    {
        public static string FormatBattleType(IBattle battle)
        {
            return battle.GetType().Name;
        }

        public static string FormatBattleType<T>() where T : IBattle
        {
            return typeof(T).Name;
        }

        public static Type GetBattleType(string name)
        {
            return TypeManager.Instance.BattleTypes[name];
        }

        public static bool IsSetSpawn(string type)
        {
            return TypeManager.Instance.BattleTypes.ContainsKey(type) && typeof(ITeamSetSpawner).IsAssignableFrom(GetBattleType(type));
        }
    }
}