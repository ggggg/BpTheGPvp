using TheGPvp.ExtendedPlayer;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;

namespace TheGPvp.ExtensionMethods
{
    public static class ExtensionPlayer
    {
        public static PlayerItem GetExtendedPlayerPvp(this ShPlayer player)
        {
            return Core.Instance.PlayerHandler.GetSafe(player.ID);
        }
    }
}