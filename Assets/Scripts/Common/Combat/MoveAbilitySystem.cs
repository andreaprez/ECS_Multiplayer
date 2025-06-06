using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace ECS_Multiplayer.Common.Combat
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct MoveAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transform, moveSpeed) in 
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<AbilityMoveSpeed>>().WithAll<Simulate>())
            {
                transform.ValueRW.Position += transform.ValueRW.Forward() * moveSpeed.ValueRO.Value * deltaTime;
            }
        }
    }
}