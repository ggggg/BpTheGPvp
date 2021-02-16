using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.AI;
using BrokeProtocol.Utility.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TheGPvp.ExtensionMethods;
using BPEssentials.ExtensionMethods;
using TheGPvp;
using TheGPvp.BattleTypes;
using static TheGPvp.ArenaManager;

namespace TheGPvp.Events
{
    public class OnDeath : IScript
    {
        [Target(GameSourceEvent.PlayerDeath, ExecutionMode.PostEvent)]
        public void OnEvent(ShPlayer player, ShPlayer attacker)
        {
            if (!player.isHuman || player.GetExtendedPlayerPvp().ActivePvp == null)
            {
                return;
            }
            player.GetExtendedPlayerPvp().ActivePvp.OnPlayerDeath(player, attacker);
        }
    }
}