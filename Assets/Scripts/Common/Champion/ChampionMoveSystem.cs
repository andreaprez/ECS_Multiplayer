using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace ECS_Multiplayer.Common.Champion
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct ChampionMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GamePlayingTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, movePosition, moveSpeed) in 
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<ChampionMoveTargetPosition>, RefRO<CharacterMoveSpeed>>()
                         .WithAll<Simulate>())
            {
                var moveTarget = movePosition.ValueRO.Value;
                moveTarget.y = transform.ValueRO.Position.y;

                if (math.distancesq(transform.ValueRO.Position, moveTarget) < 0.05f)
                    continue;

                var moveDirection = math.normalize(moveTarget - transform.ValueRO.Position);
                var moveVector = moveDirection * moveSpeed.ValueRO.Value * deltaTime;

                transform.ValueRW.Position += moveVector;
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(moveDirection, math.up());
            }
        }
    }
}