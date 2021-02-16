using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BrokeProtocol.API;

namespace TheGPvp.BattleTypes
{
    public class TypeManager
    {
        private static TypeManager _instance;

        public static TypeManager Instance => _instance ?? (_instance = new TypeManager());

        public static readonly string AllType = "*";
        public Dictionary<string, Type> BattleTypes { get; } = new Dictionary<string, Type>();

        private readonly string DataKey = "pvp.battle.name";

        public void LoadAllTypes()
        {
            foreach (var battle in from x in BPAPI.Instance.Plugins.Values
                                   from battleType in Assembly.GetAssembly(x.Plugin.GetType()).GetTypes().Where(myType =>
                               myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Battle)))
                                   select new KeyValuePair<Plugin, Type>(x.Plugin, battleType))
            {
                Core.Instance.Logger.LogInfo(battle.Value.Name);
                BattleTypes.Add(/*battle.Key.CustomData.FetchCustomData<string>(DataKey) ??*/ battle.Value.Name, battle.Value);
            }
        }
    }
}