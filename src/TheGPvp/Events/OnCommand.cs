using BPEssentials.ExtensionMethods;
using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheGPvp.ExtensionMethods;

namespace TheGPvp.Events
{
    internal class OnCommand
    {
        [Target(GameSourceEvent.PlayerLocalChatMessage, ExecutionMode.Test)]
        public bool OnLocalChatMessage(ShPlayer player, string message)
        {
            return TestMethod(player, message);
        }

        [Target(GameSourceEvent.PlayerGlobalChatMessage, ExecutionMode.Test)]
        public bool OnGlobalChatMessage(ShPlayer player, string message)
        {
            return TestMethod(player, message);
        }

        private bool TestMethod(ShPlayer player, string message)
        {
            if (!message.StartsWith("/"))
            {
                return true;
            }
            var booli = player.GetExtendedPlayerPvp().ActivePvp?.Battle?.BattleStarted;
            if (booli == false || booli == null)
            {
                return true;
            }
            player.TS("pvp_command");
            return false;
        }
    }
}