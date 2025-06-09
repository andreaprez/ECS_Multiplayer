using ECS_Multiplayer.Common.Champion;
using ECS_Multiplayer.Common.Respawn;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace ECS_Multiplayer.Common.Combat
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<GamePrefabs>();
            state.RequireForUpdate<RespawnEntityTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            
            if (!networkTime.IsFirstTimeFullyPredictingTick)
                return;
            
            var currentTick = networkTime.ServerTick;
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (transform, entity) in 
                     SystemAPI.Query<RefRW<LocalTransform>>().WithAll<DestroyEntityTag, Simulate>().WithEntityAccess())
            {
                if (state.World.IsServer())
                {
                    if (SystemAPI.HasComponent<GameOverOnDestroyTag>(entity))
                    {
                        var gameOverPrefab = SystemAPI.GetSingleton<GamePrefabs>().GameOverEntity;
                        var gameOverEntity = ecb.Instantiate(gameOverPrefab);

                        var losingTeam = SystemAPI.GetComponent<GameTeam>(entity).Value;
                        var winningTeam = losingTeam == TeamType.Blue ? TeamType.Red : TeamType.Blue;

                        ecb.SetComponent(gameOverEntity, new WinningTeam { Value = winningTeam });
                    }

                    if (SystemAPI.HasComponent<ChampionTag>(entity))
                    {
                        var networkEntity = SystemAPI.GetComponent<NetworkEntityReference>(entity).Value;
                        var respawnEntity = SystemAPI.GetSingletonEntity<RespawnEntityTag>();
                        var respawnTickCount = SystemAPI.GetComponent<RespawnTickCount>(respawnEntity).Value;

                        var respawnTick = currentTick;
                        respawnTick.Add(respawnTickCount);
                        
                        ecb.AppendToBuffer(respawnEntity, new RespawnBuffer
                        {
                            NetworkEntity = networkEntity,
                            NetworkId = SystemAPI.GetComponent<NetworkId>(networkEntity).Value,
                            RespawnTick = respawnTick
                        });
                    }
                    
                    ecb.DestroyEntity(entity);
                }
                else
                {
                    transform.ValueRW.Position = new float3(1000f, 1000f, 1000f);
                }
            }
        }
    }
}