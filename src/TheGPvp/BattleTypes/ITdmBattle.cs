using System.Collections.Generic;
using BrokeProtocol.Entities;

namespace TheGPvp.BattleTypes
{
    public interface ITdmBattle<out T> : IBattle where T : Team
    {
        T[] Teams { get; }
        T[] AliveTeams { get; }

        /// <summary>
        /// Split all the players into a team and add each team to the Teams array.
        /// </summary>
        /// <see cref="TdmBattle{T}.Teams"/>
        void PopulateTeams();

        T GetPlayerTeam(ShPlayer player);
    }
}