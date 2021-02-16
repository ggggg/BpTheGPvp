using BrokeProtocol.API;
using BrokeProtocol.Managers;
using System;
using TheGPvp.BattleTypes;

namespace TheGPvp.RegisteredEvents
{
    public class OnStarted : IScript
    {
        [Target(GameSourceEvent.ManagerStart, ExecutionMode.Event)]
        public void OnEvent(SvManager svManager)
        {
            Core.Instance.SvManager = svManager;
            TypeManager.Instance.LoadAllTypes();
            Core.Instance.ArenaManager.StartUp();
            Core.Instance.RankManager.StartUp();
        }
    }
}