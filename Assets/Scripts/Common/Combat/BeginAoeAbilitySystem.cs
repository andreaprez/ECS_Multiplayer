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
                var isOnCooldown = true;
                var currentTargetTicks = new AbilityCooldownTargetTicks();

                // Safe check in case a slow server skips over network ticks
                for (var i = 0u; i < networkTime.SimulationStepBatchSize; i++)
                {
                    var testTick = currentTick;
                    testTick.Subtract(i);

                    if (!aoe.CooldownTargetTicks.GetDataAtTick(testTick, out currentTargetTicks))
                    {
                        currentTargetTicks.AoeAbility = NetworkTick.Invalid;
                    }

                    if (currentTargetTicks.AoeAbility == NetworkTick.Invalid ||
                        !currentTargetTicks.AoeAbility.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }
                
                if (isOnCooldown)
                    continue;
                
                if (aoe.ShouldAttack)
                {
                    var newAoeAbility = ecb.Instantiate(aoe.AbilityPrefab);
                    var abilityTransform = LocalTransform.FromPositionRotationScale(aoe.AttackPosition, quaternion.identity, 15);
                    ecb.SetComponent(newAoeAbility, abilityTransform);
                    ecb.SetComponent(newAoeAbility, aoe.Team);
                    
                    if (state.WorldUnmanaged.IsServer())
                        continue;
                    
                    var newCooldownTargetTick = currentTick;
                    newCooldownTargetTick.Add(aoe.CooldownTicks);
                    currentTargetTicks.AoeAbility = newCooldownTargetTick;

                    // To avoid de-sync with server, we need to set the current tick as the next tick
                    var nextTick = currentTick;
                    nextTick.Add(1u);
                    currentTargetTicks.Tick = nextTick;

                    aoe.CooldownTargetTicks.AddCommandData(currentTargetTicks);
                }
            }
        }
    }
}