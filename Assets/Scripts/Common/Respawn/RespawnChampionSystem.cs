using System;
using ECS_Multiplayer.Common.Champion;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace ECS_Multiplayer.Common.Respawn
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class RespawnChampionSystem : SystemBase
    {
        public Action<int> OnUpdateRespawnCountdown;
        public Action OnRespawn;

        protected override void OnCreate()
        {
            RequireForUpdate<NetworkTime>();
            RequireForUpdate<GamePrefabs>();
        }

        protected override void OnStartRunning()
        {
            if (SystemAPI.HasSingleton<RespawnEntityTag>())
                return;

            var respawnPrefab = SystemAPI.GetSingleton<GamePrefabs>().RespawnEntity;
            EntityManager.Instantiate(respawnPrefab);
        }

        protected override void OnUpdate()
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick)
                return;
            
            var currentTick = networkTime.ServerTick;

            var isServer = World.IsServer();

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var respawnBuffer in 
                     SystemAPI.Query<DynamicBuffer<RespawnBuffer>>().WithAll<RespawnTickCount, Simulate>())
            {
                var respawnsToCleanup = new NativeList<int>(Allocator.Temp);
                
                for (var i = 0; i < respawnBuffer.Length; i++)
                {
                    var currentRespawn = respawnBuffer[i];
                    if (currentTick.Equals(currentRespawn.RespawnTick) || currentTick.IsNewerThan(currentRespawn.RespawnTick))
                    {
                        if (isServer)
                        {
                            var networkId = SystemAPI.GetComponent<NetworkId>(currentRespawn.NetworkEntity).Value;
                            var playerSpawnInfo = SystemAPI.GetComponent<PlayerSpawnInfo>(currentRespawn.NetworkEntity);

                            var championPrefab = SystemAPI.GetSingleton<GamePrefabs>().Champion;
                            var newChampion = ecb.Instantiate(championPrefab);
                            
                            ecb.SetComponent(newChampion, new GhostOwner { NetworkId = networkId });
                            ecb.SetComponent(newChampion, new GameTeam { Value = playerSpawnInfo.Team });
                            var newTransform = LocalTransform.FromPositionRotationScale(playerSpawnInfo.SpawnPosition, quaternion.identity, 4);
                            ecb.SetComponent(newChampion, newTransform);
                            ecb.SetComponent(newChampion, new ChampionMoveTargetPosition { Value = playerSpawnInfo.SpawnPosition });
                            ecb.AppendToBuffer(currentRespawn.NetworkEntity, new LinkedEntityGroup { Value = newChampion });
                            ecb.SetComponent(newChampion, new NetworkEntityReference { Value = currentRespawn.NetworkEntity });
                            
                            respawnsToCleanup.Add(i);
                        }
                        else
                        {
                            OnRespawn?.Invoke();
                        }
                    }
                    else if (!isServer)
                    {
                        if (SystemAPI.TryGetSingleton<NetworkId>(out var networkId))
                        {
                            if (networkId.Value == currentRespawn.NetworkId)
                            {
                                var ticksToRespawn = currentRespawn.RespawnTick.TickIndexForValidTick - currentTick.TickIndexForValidTick;
                                var simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
                                var secondsToStart = (int)math.ceil((float)ticksToRespawn / simulationTickRate);
                                OnUpdateRespawnCountdown?.Invoke(secondsToStart);
                            }
                        }
                    }
                }

                foreach (var respawnIndex in respawnsToCleanup)
                {
                    respawnBuffer.RemoveAt(respawnIndex);
                }
            }
            
            ecb.Playback(EntityManager);
        }
    }
}