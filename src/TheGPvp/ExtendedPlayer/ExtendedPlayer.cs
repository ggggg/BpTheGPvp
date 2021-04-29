using BPEssentials.Models;
using BPEssentials.Enums;
using BPEssentials.ExtensionMethods;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using BrokeProtocol.Managers;
using BrokeProtocol.Utility.Networking;
using TheGPvp.ArenaTypes;
using TheGPvp.Configuration.Models;
using TheGPvp.ExtensionMethods;

namespace TheGPvp.ExtendedPlayer
{
    [Serializable]
    public class PlayerItem : BPCoreLib.Abstractions.ExtendedPlayer
    {
        public PlayerItem(ShPlayer player) : base(player)
        {
            this.player = player;
        }

        public Arena PvpInvite { get; set; }

        public Dictionary<ShItem, int> SavedItems { get; set; }

        public void ClearCrimes()
        {
            player.ClearCrimes();
            player.svPlayer.Send(SvSendType.Self, Channel.Reliable, ClPacket.ClearCrimes, player.ID);
        }

        public Arena ActivePvp => Core.Instance.ArenaManager.Arenas.FirstOrDefault(x =>
                   x.Battle != null && ((x.Battle.BattleStarted && x.Battle.AlivePlayers.Contains(player)) ||
                                        (!x.Battle.BattleStarted && x.Battle.Players.Contains(player))));

        public bool CrimeLoop { get; set; }

        private readonly ShPlayer player;

        public void AddCrimes()
        {
            if (player == null)
            {
                return;
            }
            CrimeLoop = true;
            player.ClearCrimes();
            player.svPlayer.Send(SvSendType.Self, Channel.Reliable, ClPacket.ClearCrimes, player.ID);
            player.AddCrime(CrimeIndex.Bombing, null);
            player.svPlayer.Send(SvSendType.Self, Channel.Reliable, ClPacket.AddCrime, CrimeIndex.Bombing, 0);
            Core.Instance.StartMethodTimer(60, () =>
            {
                if (!Core.Instance.SvManager.connectedPlayers.ContainsValue(player) || ActivePvp == null)
                {
                    CrimeLoop = false;
                    return;
                }
                AddCrimes();
            });
        }

        public void TpPlayerToLastLocation()
        {
            var extendedPlayer = player.GetExtendedPlayer();
            if (extendedPlayer != null)
            {
                if (extendedPlayer.LastLocation.HasPositionSet() && extendedPlayer.LastLocation.PlaceIndex <=
                    (Core.Instance.SvManager.fixedPlaces.Count + Core.Instance.SvManager.apartments.Count))
                {
                    Core.Instance.Logger.LogInfo(
                        $"tping {player.username.CleanerMessage()} to {extendedPlayer.LastLocation.Position.x}, {extendedPlayer.LastLocation.Position.y}, {extendedPlayer.LastLocation.Position.z} int {extendedPlayer.LastLocation.PlaceIndex}");
                    player.svPlayer.SvRestore(extendedPlayer.LastLocation.Position,
                        extendedPlayer.LastLocation.Rotation, extendedPlayer.LastLocation.PlaceIndex);
                    return;
                }

                var newSpawn = Core.Instance.SvManager.spawnLocations.GetRandom().transform;
                player.svPlayer.SvRestore(newSpawn.position, newSpawn.rotation, newSpawn.parent.GetSiblingIndex());
                return;
            }
            Core.Instance.SvManager.TryGetUserData(player.username, out var playerData);

            Core.Instance.StartMethodTimer(1, () =>
                {
                    var newSpawn = Core.Instance.SvManager.spawnLocations.GetRandom().transform;
                    playerData.Character.Position = newSpawn.position;
                    playerData.Character.Rotation = newSpawn.rotation;
                    playerData.Character.PlaceIndex = newSpawn.parent.GetSiblingIndex();
                    Core.Instance.SvManager.database.Users.Upsert(playerData);
                });
        }

        public void TpToSpawnSave(ArenaManager.Spawn spawn)
        {
            player.GetExtendedPlayer().ResetAndSavePosition(spawn.FormatSpawn(), spawn.Rotation, spawn.Index);
        }

        public void TpToSpawn(ArenaManager.Spawn s)
        {
            player.svPlayer.SvRestore(s.FormatSpawn(), s.Rotation, s.Index);
        }

        public void RemoveAllItemsSave()
        {
            SavedItems = player.myItems.Values.ToDictionary(x => x.item, x => x.count);
            RemoveItems(player.myItems.Values.ToList());
        }

        public void RemoveItems(List<InventoryItem> items)
        {
            foreach (var item in items)
            {
                player.TransferItem(DeltaInv.RemoveFromMe, item.item.index, item.count, true);
            }
        }

        public void GiveItemsBack()
        {
            if (SavedItems == null)
            {
                return;
            }
            RemoveItems(player.myItems.Values.ToList());
            foreach (var item in SavedItems)
            {
                player.TransferItem(DeltaInv.AddToMe, item.Key, item.Value, true);
            }
            SavedItems = null;
        }

        public void GiveKitSave(List<KitModel> kits)
        {
            RemoveAllItemsSave();
            GiveKit(kits);
        }

        public void GiveKit(List<KitModel> kits)
        {
            foreach (var kit in kits)
            {
                player.TransferItem(DeltaInv.AddToMe,
                    SceneManager.Instance.entityCollection
                        .FirstOrDefault(x => x.Value is ShItem && x.Value.name == kit.Item).Value.index, kit.Amount,
                    true);
            }
        }
    }
}