using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;

namespace TheGPvp.Events
{
    public class OnLogin : IScript
    {
        [Target(GameSourceEvent.PlayerInitialize, ExecutionMode.Event)]
        public void OnEvent(ShPlayer player)
        {
            if (!player.isHuman)
            {
                return;
            }
            Core.Instance.PlayerHandler.AddOrReplace(player);
        }
    }
}