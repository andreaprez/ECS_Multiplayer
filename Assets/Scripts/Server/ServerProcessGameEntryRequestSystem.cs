using ECS_Multiplayer.Common;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace ECS_Multiplayer.Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntryRequestSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<TeamRequest, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
            state.RequireForUpdate<GamePrefabs>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var championPrefab = SystemAPI.GetSingleton<GamePrefabs>().Champion;

            foreach (var (teamRequest, requestSource, requestEntity) in 
                     SystemAPI.Query<RefRO<TeamRequest>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                ecb.DestroyEntity(requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.ValueRO.SourceConnection);

                var requestedTeamType = teamRequest.ValueRO.Value;

                if (requestedTeamType == TeamType.AutoAssign)
                {
                    requestedTeamType = TeamType.Blue;
                }

                var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.ValueRO.SourceConnection).Value;
                Debug.Log($"Server is assigning Client ID: {clientId} to the {requestedTeamType.ToString()} team.");

                var newChampion = ecb.Instantiate(championPrefab);
                ecb.SetName(newChampion, "Champion");
                var spawnPosition = new float3(0, 1, 0);
                var newTransform = LocalTransform.FromPosition(spawnPosition);
                ecb.SetComponent(newChampion, newTransform);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}