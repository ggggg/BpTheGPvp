using BPEssentials.ExtensionMethods;
using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheGPvp.ArenaManager;

namespace TheGPvp.Commands
{
    public class Reload : IScript
    {
        public Reload()
        {
            List<string> cmds = new List<string> { "pvpreload" };
            CommandHandler.RegisterCommand(cmds, new Action<ShPlayer>(OnCommandInvoke));
            Core.Instance.Logger.LogInfo("Registered " + nameof(Reload));
        }

        public void OnCommandInvoke(ShPlayer player)
        {
            Core.Instance.OnReloadRequestAsync();
            player.TS("Reloaded");
        }
    }
}