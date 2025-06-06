using ECS_Multiplayer.Common.Champion;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace ECS_Multiplayer.Common.Combat
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct DamageOnTriggerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var damageOnTriggerJob = new DamageOnTriggerJob
            {
                DamageOnTriggerLookup = SystemAPI.GetComponentLookup<DamageOnTrigger>(true),
                TeamLookup = SystemAPI.GetComponentLookup<GameTeam>(true),
                AlreadyDamagedBufferLookup = SystemAPI.GetBufferLookup<AlreadyDamagedEntityBuffer>(true),
                DamageBufferLookup = SystemAPI.GetBufferLookup<DamageBuffer>(true),
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            };
            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = damageOnTriggerJob.Schedule(simulationSingleton, state.Dependency);
        }
    }
    
    public struct DamageOnTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<DamageOnTrigger> DamageOnTriggerLookup;
        [ReadOnly] public ComponentLookup<GameTeam> TeamLookup;
        [ReadOnly] public BufferLookup<AlreadyDamagedEntityBuffer> AlreadyDamagedBufferLookup;
        [ReadOnly] public BufferLookup<DamageBuffer> DamageBufferLookup;

        public EntityCommandBuffer ECB;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity damageDealingEntity;
            Entity damageReceivingEntity;

            if (DamageBufferLookup.HasBuffer(triggerEvent.EntityA) &&
                DamageOnTriggerLookup.HasComponent(triggerEvent.EntityB))
            {
                damageReceivingEntity = triggerEvent.EntityA;
                damageDealingEntity = triggerEvent.EntityB;
            }
            else if (DamageOnTriggerLookup.HasComponent(triggerEvent.EntityB) &&
                     DamageBufferLookup.HasBuffer(triggerEvent.EntityA))
            {
                damageDealingEntity = triggerEvent.EntityA;
                damageReceivingEntity = triggerEvent.EntityB;
            }
            else
            {
                return;
            }

            var alreadyDamagedBuffer = AlreadyDamagedBufferLookup[damageDealingEntity];
            foreach (var alreadyDamagedEntity in alreadyDamagedBuffer)
            {
                if (alreadyDamagedEntity.Value.Equals(damageReceivingEntity))
                    return;
            }

            if (TeamLookup.TryGetComponent(damageDealingEntity, out var damageDealingTeam) &&
                TeamLookup.TryGetComponent(damageReceivingEntity, out var damageReceivingTeam))
            {
                if (damageDealingTeam.Value == damageReceivingTeam.Value)
                    return;
            }

            var damageOnTrigger = DamageOnTriggerLookup[damageDealingEntity];
            ECB.AppendToBuffer(damageReceivingEntity, new DamageBuffer { Value = damageOnTrigger.Value });
            ECB.AppendToBuffer(damageDealingEntity, new AlreadyDamagedEntityBuffer() { Value = damageReceivingEntity });
        }
    }
}