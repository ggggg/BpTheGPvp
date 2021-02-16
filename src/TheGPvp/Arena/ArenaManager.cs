using System;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using BPCoreLib.Serializable;
using TheGPvp.ArenaTypes;
using TheGPvp.BattleTypes;
using UnityEngine;

namespace TheGPvp
{
    public class ArenaManager
    {
        public string ArenasTable { get; set; } = Core.Instance.Info.GroupNamespace;

        public Dictionary<string, Arena> GlobalPvp { get; set; } = new Dictionary<string, Arena>();

        public ILiteCollection<ArenaSerializable> ArenaCollection { get; set; }

        public List<Arena> Arenas { get; set; } = new List<Arena>();

        [Serializable]
        public class ArenaSerializable
        {
            [BsonId(true)]
            public string Name { get; set; }

            public List<BattleSpawn> Spawns { get; set; } = new List<BattleSpawn>();

            public LobbySpawn Lobby { get; set; }

            public string Type { get; set; }
        }

        [Serializable]
        public class LobbySpawn
        {
            public float X { get; set; }

            public float Y { get; set; }

            public float Z { get; set; }

            public int Index { get; set; }

            public Quaternion Rotation { get; set; }

            public static implicit operator Spawn(LobbySpawn exists)
            {
                return new Spawn
                {
                    X = exists.X,
                    Y = exists.Y,
                    Z = exists.Z,
                    Index = exists.Index,
                    Rotation = exists.Rotation
                };
            }
        }

        [Serializable]
        public struct Spawn
        {
            public float X { get; set; }

            public float Y { get; set; }

            public float Z { get; set; }

            public int Index { get; set; }

            public Quaternion Rotation { get; set; }
        }

        [Serializable]
        public class BattleSpawn
        {
            public string SpawnType { get; set; } = TypeManager.AllType;

            public string TeamName { get; set; }

            public float X { get; set; }

            public float Y { get; set; }

            public float Z { get; set; }

            public int Index { get; set; }

            public Quaternion Rotation { get; set; }

            public static implicit operator Spawn(BattleSpawn exists)
            {
                return new Spawn
                {
                    X = exists.X,
                    Y = exists.Y,
                    Z = exists.Z,
                    Index = exists.Index,
                    Rotation = exists.Rotation
                };
            }
        }

        public void StartUp()
        {
            if (!Core.Instance.SvManager.database.LiteDB.CollectionExists(ArenasTable))
            {
                Core.Instance.Logger.LogWarning("Did not find Arena Table: " + ArenasTable);
            }

            ArenaCollection = Core.Instance.SvManager.database.LiteDB.GetCollection<ArenaSerializable>(ArenasTable);
            foreach (var arena in ArenaCollection.FindAll().ToArray())
            {
                Core.Instance.Logger.Log($"Loading Data for: {arena.Name} ");
                if (Arenas.FirstOrDefault(x => x.ArenaSerializable == arena) != null)
                {
                    Core.Instance.Logger.LogWarning($"Arena {arena.Name} is duplicated! Deleting.");
                    ArenaCollection.Delete(arena.Name);
                    continue;
                }

                Arenas.Add(new Arena(arena));
            }
        }

        public Arena GetRandomArena(int numberOfTeams, string type)
        {
            var goodArenas = (from x in Core.Instance.ArenaManager.Arenas
                              where !x.InUse &&
                                    x.GetValidSpawns(type).Count >= numberOfTeams &&
                                    x.ArenaSerializable.Lobby != null &&
                                   (type == TypeManager.AllType || x.ArenaSerializable.Type == TypeManager.AllType || x.ArenaSerializable.Type == type)
                              select x).ToList();
            Core.Instance.Logger.LogInfo(goodArenas.Count + "");
            return !goodArenas.Any() ? null : goodArenas[UnityEngine.Random.Range(0, goodArenas.Count - 1)];
        }

        public Arena GetRandomArena(string type)
        {
            return GetRandomArena(2, type);
        }
    }
}