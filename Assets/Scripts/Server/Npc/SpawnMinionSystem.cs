using ECS_Multiplayer.Common;
using ECS_Multiplayer.Common.Champion;
using ECS_Multiplayer.Common.Npc;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECS_Multiplayer.Server.Npc
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SpawnMinionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GamePrefabs>();
            state.RequireForUpdate<MinionPathContainers>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach (var minionSpawnAspect in SystemAPI.Query<MinionSpawnAspect>())
            {
                minionSpawnAspect.DecrementTimers(deltaTime);
                if (minionSpawnAspect.ShouldSpawn)
                {
                    SpawnOnEachLane(ref state);
                    minionSpawnAspect.CountSpawnedInWave++;
                    if (minionSpawnAspect.IsWaveSpawned)
                    { 
                        minionSpawnAspect.ResetSpawnCounter();
                        minionSpawnAspect.ResetWaveTimer(); 
                        minionSpawnAspect.ResetMinionTimer(); 
                    }
                    else
                    {
                        minionSpawnAspect.ResetMinionTimer();
                    }
                }
            }
        }

        private void SpawnOnEachLane(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var minionPrefab = SystemAPI.GetSingleton<GamePrefabs>().Minion;
            var pathContainers = SystemAPI.GetSingleton<MinionPathContainers>();

            var topLane = SystemAPI.GetBuffer<MinionPathPosition>(pathContainers.TopLane);
            SpawnOnLane(ecb, minionPrefab, topLane);
            var middleLane = SystemAPI.GetBuffer<MinionPathPosition>(pathContainers.MiddleLane);
            SpawnOnLane(ecb, minionPrefab, middleLane);
            var bottomLane = SystemAPI.GetBuffer<MinionPathPosition>(pathContainers.BottomLane);
            SpawnOnLane(ecb, minionPrefab, bottomLane);
        }

        private void SpawnOnLane(EntityCommandBuffer ecb, Entity minionPrefab, DynamicBuffer<MinionPathPosition> lane)
        {
            var newBlueMinion = ecb.Instantiate(minionPrefab);
            for (var i = 0; i < lane.Length; i++)
            {
                ecb.AppendToBuffer(newBlueMinion, lane[i]);
            }
            var blueSpawnTransform = LocalTransform.FromPosition(lane[0].Value);
            ecb.SetComponent(newBlueMinion, blueSpawnTransform);
            ecb.SetComponent(newBlueMinion, new GameTeam {Value = TeamType.Blue});
            
            var newRedMinion = ecb.Instantiate(minionPrefab);
            for (var i = lane.Length - 1; i >= 0; i--)
            {
                ecb.AppendToBuffer(newBlueMinion, lane[i]);
            }
            var redSpawnTransform = LocalTransform.FromPosition(lane[^1].Value);
            ecb.SetComponent(newRedMinion, redSpawnTransform);
            ecb.SetComponent(newRedMinion, new GameTeam {Value = TeamType.Red});
        }
    }
}