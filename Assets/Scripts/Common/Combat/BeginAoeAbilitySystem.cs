using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace ECS_Multiplayer.Common.Combat
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginAoeAbilitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick)
                return;
            var currentTick = networkTime.ServerTick;

            foreach (var aoe in SystemAPI.Query<AoeAspect>().WithAll<Simulate>())
            {
                if (aoe.ShouldAttack)
                {
                    var newAoeAbility = ecb.Instantiate(aoe.AbilityPrefab);
                    var abilityTransform = LocalTransform.FromPositionRotationScale(aoe.AttackPosition, quaternion.identity, 15);
                    ecb.SetComponent(newAoeAbility, abilityTransform);
                    ecb.SetComponent(newAoeAbility, aoe.Team);
                }
            }
        }
    }
}