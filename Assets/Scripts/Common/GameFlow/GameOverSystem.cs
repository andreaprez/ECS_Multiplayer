using System;
using Unity.Entities;

namespace ECS_Multiplayer.Common
{
    public partial class GameOverSystem : SystemBase
    {
        public Action<TeamType> OnGameOver;

        protected override void OnCreate()
        {
            RequireForUpdate<GamePlayingTag>();
            RequireForUpdate<GameOverTag>();
        }

        protected override void OnUpdate()
        {
            var gameOverEntity = SystemAPI.GetSingletonEntity<GameOverTag>();
            var winningTeam = SystemAPI.GetComponent<WinningTeam>(gameOverEntity).Value;
            OnGameOver?.Invoke(winningTeam);
            
            var gamePlayingEntity = SystemAPI.GetSingletonEntity<GamePlayingTag>();
            EntityManager.DestroyEntity(gamePlayingEntity);

            Enabled = false;
        }
    }
}