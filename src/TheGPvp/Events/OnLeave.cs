using BrokeProtocol.API;
using BrokeProtocol.Entities;
using TheGPvp.ExtensionMethods;
using TheGPvp.ArenaTypes;

namespace TheGPvp.Events
{
    public class OnLeave : IScript
    {
        [Target(GameSourceEvent.PlayerDestroy, ExecutionMode.Event)]
        public void OnEvent(ShPlayer player)
        {
            if (!player.isHuman || player.GetExtendedPlayerPvp().ActivePvp == null) return;
            var arena = player.GetExtendedPlayerPvp().ActivePvp;
            arena.DisconnectPlayer(player);
            Core.Instance.PlayerHandler.Remove(player.ID);
        }
    }
}