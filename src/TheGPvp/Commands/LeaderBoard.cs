using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGPvp.Commands
{
    public class LeaderBoard : IScript
    {
        public LeaderBoard()
        {
            List<string> cmds = new List<string> { "lb", "leaderboard" };
            CommandHandler.RegisterCommand(cmds, new Action<ShPlayer>(OnCommandInvoke));
            Core.Instance.Logger.LogInfo("Registered " + nameof(LeaderBoard));
        }

        public void OnCommandInvoke(ShPlayer player)
        {
            var j = 0;
            var message = new StringBuilder();
            Core.Instance.Logger.Log((++j) + "");
            var i = 0;
            Core.Instance.Logger.Log((++j) + " " + (Core.Instance.RankManager == null));
            Core.Instance.RankManager.GetTopX().ForEach(x =>
            {
                message.AppendLine(
                    $"{++i}: {x.Username} with {x.getPlayerWins(): 0} Wins, And {Math.Round(x.WinLossRatio() * 100f)}% Win/Loss Ratio");
            });
            Core.Instance.Logger.Log((++j) + "");
            var playerRank = player.GetRankedPlayer();
            Core.Instance.Logger.Log((++j) + "");
            if (playerRank != null)
            {
                message.AppendLine($"\n\nYour rank: {playerRank.PlayerRank()}: with {playerRank.getPlayerWins()} Wins, And {Convert.ToInt32(playerRank.WinLossRatio() * 100f)}% Win/Loss Ratio");
            }
            Core.Instance.Logger.Log((++j) + "");
            player.svPlayer.SendTextMenu("Top pvp players", message.ToString());
            Core.Instance.Logger.Log((++j) + "");
        }
    }
}