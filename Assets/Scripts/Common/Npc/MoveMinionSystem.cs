using ECS_Multiplayer.Common.Champion;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace ECS_Multiplayer.Common.Npc
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct MoveMinionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, pathPositions, pathIndex, moveSpeed) in 
                     SystemAPI.Query<RefRW<LocalTransform>, DynamicBuffer<MinionPathPosition>, RefRW<MinionPathIndex>, RefRO<CharacterMoveSpeed>>().WithAll<Simulate>())
            { 
                var targetPosition = pathPositions[pathIndex.ValueRO.Value].Value;
                if (math.distance(targetPosition, transform.ValueRO.Position) <= 1.5f)
                {
                    if (pathIndex.ValueRO.Value >= pathPositions.Length - 1)
                        continue;

                    pathIndex.ValueRW.Value++;
                    targetPosition = pathPositions[pathIndex.ValueRO.Value].Value;
                }
                targetPosition.y = transform.ValueRO.Position.y;
                
                var heading = math.normalizesafe(targetPosition - transform.ValueRO.Position);
                
                transform.ValueRW.Position += heading * moveSpeed.ValueRO.Value * deltaTime;
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(heading, math.up());
            }
        }
    }
}