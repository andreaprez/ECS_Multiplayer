﻿using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace ECS_Multiplayer.Common.Combat
{
    public partial struct InitializeDestroyOnTimerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (destroyOnTimer, entity) in 
                     SystemAPI.Query<RefRO<DestroyOnTimer>>().WithNone<DestroyAtTick>().WithEntityAccess())
            {
                var lifetimeInTicks = (uint)(destroyOnTimer.ValueRO.Value * simulationTickRate);
                var targetTick = currentTick;
                targetTick.Add(lifetimeInTicks);
                ecb.AddComponent(entity, new DestroyAtTick { Value = targetTick });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}