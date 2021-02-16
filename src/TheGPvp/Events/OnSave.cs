using BrokeProtocol.API;
using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using System;
using TheGPvp;

namespace BPPlasmaGangs.RegisteredEvents
{
    public class OnSave : IScript
    {
        [Target(GameSourceEvent.ManagerSave, ExecutionMode.Event)]
        public void OnEvent(SvManager svManager)
        {
            Core.Instance.SvManager = svManager;
            foreach (var arena in Core.Instance.ArenaManager.Arenas)
            {
                arena.Save();
            }
        }
    }
}