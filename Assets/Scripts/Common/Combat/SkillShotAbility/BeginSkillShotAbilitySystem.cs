﻿using ECS_Multiplayer.Client.UI;
using ECS_Multiplayer.Common.Champion;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace ECS_Multiplayer.Common.Combat
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginSkillShotAbilitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick)
                return;
            
            var currentTick = networkTime.ServerTick;

            foreach (var skillShot in 
                     SystemAPI.Query<SkillShotAspect>().WithAll<Simulate>().WithNone<AimSkillShotTag>())
            {
                var isOnCooldown = true;

                // Safe check in case a slow server skips over network ticks
                for (var i = 0u; i < networkTime.SimulationStepBatchSize; i++)
                {
                    var testTick = currentTick;
                    testTick.Subtract(i);

                    if (!skillShot.CooldownTargetTicks.GetDataAtTick(testTick, out var currentTargetTicks))
                    {
                        currentTargetTicks.SkillShotAbility = NetworkTick.Invalid;
                    }

                    if (currentTargetTicks.SkillShotAbility == NetworkTick.Invalid ||
                        !currentTargetTicks.SkillShotAbility.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }
                
                if (isOnCooldown || !skillShot.ShouldAttack)
                    continue;
                    
                ecb.AddComponent<AimSkillShotTag>(skillShot.ChampionEntity);

                if (state.WorldUnmanaged.IsServer() ||
                    !SystemAPI.HasComponent<OwnerChampionTag>(skillShot.ChampionEntity))
                    continue;

                var skillShotUIPrefab = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().SkillShotAim;
                var newSkillShotUI = Object.Instantiate(skillShotUIPrefab, skillShot.AttackPosition, Quaternion.identity);
                ecb.AddComponent(skillShot.ChampionEntity, new SkillShotAimUIReference { Value = newSkillShotUI });
            }

            foreach (var skillShot in
                     SystemAPI.Query<SkillShotAspect>().WithAll<AimSkillShotTag, Simulate>())
            {
                if (!skillShot.ConfirmAttack)
                    continue;

                var skillShotAbility = ecb.Instantiate(skillShot.AbilityPrefab);
                var abilityTransform = LocalTransform.FromPositionRotationScale(skillShot.SpawnTransform.Position, 
                    skillShot.SpawnTransform.Rotation, 1);

                ecb.SetComponent(skillShotAbility, abilityTransform);
                ecb.SetComponent(skillShotAbility, skillShot.Team);
                ecb.RemoveComponent<AimSkillShotTag>(skillShot.ChampionEntity);
                
                if (state.WorldUnmanaged.IsServer())
                    continue;

                skillShot.CooldownTargetTicks.GetDataAtTick(currentTick, out var currentTargetTicks);

                var newCooldownTargetTick = currentTick;
                newCooldownTargetTick.Add(skillShot.CooldownTicks);
                currentTargetTicks.SkillShotAbility = newCooldownTargetTick;

                // To avoid de-sync with server, we need to set the current tick as the next tick
                var nextTick = currentTick;
                nextTick.Add(1u);
                currentTargetTicks.Tick = nextTick;

                skillShot.CooldownTargetTicks.AddCommandData(currentTargetTicks);
            }

            foreach (var (abilityInput, skillShotAimUIReference, entity) in 
                     SystemAPI.Query<RefRO<AbilityInput>, SkillShotAimUIReference>().WithAll<Simulate>().WithEntityAccess())
            {
                if (!abilityInput.ValueRO.ConfirmSkillShotAbility.IsSet)
                    continue;
                
                Object.Destroy(skillShotAimUIReference.Value);
                ecb.RemoveComponent<SkillShotAimUIReference>(entity);
            }
            
            foreach (var (skillShotAimUIReference, entity) in 
                     SystemAPI.Query<SkillShotAimUIReference>().WithAll<Simulate>().WithNone<LocalTransform>().WithEntityAccess())
            {
                Object.Destroy(skillShotAimUIReference.Value);
                ecb.RemoveComponent<SkillShotAimUIReference>(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}