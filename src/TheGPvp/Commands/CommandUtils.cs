using BPEssentials.ExtensionMethods;
using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheGPvp.ArenaTypes;
using TheGPvp.BattleTypes;
using TheGPvp.ExtensionMethods;

namespace TheGPvp.Commands
{
    public static class CommandUtils
    {
        public static bool CheckCommandValidity(ShPlayer player)
        {
            const string commandName = "pvp";
            if (!player.GetExtendedPlayer().EnabledBypass)
            {
                if (player.IsDead)
                {
                    player.TS("command_failed_crimes", commandName);
                    return false;
                }

                if (player.IsRestrained)
                {
                    player.TS("command_failed_cuffed", commandName);
                    return false;
                }

                if (player.svPlayer.job.info.shared.jobIndex == BPAPI.Instance.PrisonerIndex)
                {
                    player.TS("command_failed_jail", commandName);
                    return false;
                }

                if (player.wantedLevel != 0)
                {
                    player.TS("command_failed_crimes", commandName);
                    return false;
                }
            }
            return true;
        }

        public static bool InPvp(ShPlayer player)
        {
            if (player.GetExtendedPlayerPvp().ActivePvp == null)
            {
                return false;
            }
            player.TS("already_inbattle");
            return true;
        }

        public static Arena GetInviteArena<T>(ShPlayer player) where T : IBattle
        {
            var arena = player.GetExtendedPlayerPvp().PvpInvite;
            if (arena != null)
            {
                return arena;
            }
            var battleType = Utils.FormatBattleType<T>();
            return Core.Instance.ArenaManager.GlobalPvp.ContainsKey(battleType)
                ? Core.Instance.ArenaManager.GlobalPvp[battleType]
                : null;
        }

        public static bool TryGetRandomArena<T>(out Arena arena) where T : IBattle
        {
            arena = Core.Instance.ArenaManager.GetRandomArena(Utils.FormatBattleType<T>());
            return arena != null;
        }
    }
}