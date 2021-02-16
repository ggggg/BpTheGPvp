using System;
using System.Collections;
using BrokeProtocol.LiteDB;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using BrokeProtocol.Client.UI;
using BrokeProtocol.Entities;
using TheGPvp;
using TheGPvp.BattleTypes;
using TheGPvp.ExtensionMethods;

namespace TheGPvp
{
    public class RankManager
    {
        public List<User> WinsList { get; set; }

        public string RankedTable { get; set; } = Core.Instance.Info.GroupNamespace + "_Ranked";

        public LiteDB.ILiteCollection<PlayerSerilizable> RankedCollection { get; set; } = null;

        public class PlayerSerilizable
        {
            public PlayerSerilizable(string username)
            {
                Username = username;
                Battles = new List<BattleSerilizable>();
            }

            [BsonId(true)]
            public string Username { get; set; }

            // Stores each battle.
            public List<BattleSerilizable> Battles { get; set; }
        }

        public class BattleSerilizable
        {
            public long TimeStamp { get; set; }

            public bool Won { get; set; }

            public BattleSerilizable(long timeStamp, bool won)
            {
                TimeStamp = timeStamp;
                Won = won;
            }

            public BattleSerilizable(bool won) : this(DateTimeOffset.Now.ToUnixTimeSeconds(), won)
            {
            }
        }

        public void StartUp()
        {
            if (!Core.Instance.SvManager.database.LiteDB.CollectionExists(RankedTable))
            {
                Core.Instance.Logger.LogWarning("Did not find Table:" + RankedTable);
            }

            RankedCollection = Core.Instance.SvManager.database.LiteDB.GetCollection<PlayerSerilizable>(RankedTable);
        }

        public List<PlayerSerilizable> GetTopX(int count = 10)
        {
            Core.Instance.Logger.Log("Get top x");
            return RankedCollection.FindAll().OrderByDescending(x => x.getPlayerWins()).Take(count).ToList();
        }
    }

    public static class PlayerExtensions
    {
        public static int PlayerRank(this RankManager.PlayerSerilizable player)
        {
            return Core.Instance.RankManager.RankedCollection.Count(x => x.Battles.Count(y => y.Won) > player.getPlayerWins());
        }

        public static float WinLossRatio(this RankManager.PlayerSerilizable player)
        {
            return ((float)player.getPlayerWins()) / player.Battles.Count(y => !y.Won);
        }

        public static int getPlayerWins(this RankManager.PlayerSerilizable player) => player.Battles.Count(y => y.Won);

        public static bool ReadyForRanked(this ShPlayer player)
        {
            float invScore = 0;
            foreach (var x in player.myItems)
            {
                if (x.Value.item is ShWeapon)
                {
                    invScore += x.Value.item.value;
                }
            }
            // TODO think of better value
            return invScore > 3000;
        }

        public static RankManager.PlayerSerilizable GetRankedPlayer(this ShPlayer player) =>
            Core.Instance.RankManager.RankedCollection.FindById(player.username);

        public static void RegisterBattle(this ShPlayer player, bool won) =>
            player.RegisterBattle(new RankManager.BattleSerilizable(won));

        public static void RegisterBattle(this ShPlayer player, RankManager.BattleSerilizable battle)
        {
            var ranked = player.GetRankedPlayer();
            if (ranked == null)
            {
                ranked = new RankManager.PlayerSerilizable(player.username);
                ranked.Battles.Add(battle);
                Core.Instance.RankManager.RankedCollection.Insert(ranked);
                return;
            }
            ranked.Battles.Add(battle);
            Core.Instance.RankManager.RankedCollection.Upsert(ranked);
        }
    }
}