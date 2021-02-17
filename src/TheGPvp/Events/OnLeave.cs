using BrokeProtocol.API;
using BrokeProtocol.Entities;
using TheGPvp.ExtensionMethods;
using TheGPvp.ArenaTypes;

namespace TheGPvp.Events
{
    public class OnLeave : IScript
    {
        [Target(GameSourceEvent.PlayerDestroy, ExecutionMode.Test)]
        public bool TestEvent(ShPlayer player)
        {
            if (!player.isHuman || player.GetExtendedPlayerPvp().ActivePvp == null)
            {
                return false;
            }
            var arena = player.GetExtendedPlayerPvp().ActivePvp;
            arena.DisconnectPlayer(player);
            Core.Instance.PlayerHandler.Remove(player.ID);
            return false;
        }

        [Target(GameSourceEvent.PlayerDestroy, ExecutionMode.PostEvent)]
        public void PostEvent(ShPlayer player)
        {
            if (!player.isHuman)
            {
                return;
            }
            Core.Instance.PlayerHandler.Remove(player.ID);
        }
    }
}