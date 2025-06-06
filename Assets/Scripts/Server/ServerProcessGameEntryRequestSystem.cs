using ECS_Multiplayer.Common;
using ECS_Multiplayer.Common.Champion;
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

                float3 spawnPosition;
                switch (requestedTeamType)
                {
                    case TeamType.Blue:
                        spawnPosition = new float3(-75f, 3f, -75f);
                        break;
                    case TeamType.Red:
                        spawnPosition = new float3(75f, 3f, 75f);
                        break;
                    default:
                        continue;
                }

                var newChampion = ecb.Instantiate(championPrefab);
                ecb.SetName(newChampion, "Champion");
                var newTransform = LocalTransform.FromPositionRotationScale(spawnPosition, quaternion.identity, 4);
                ecb.SetComponent(newChampion, newTransform);
                ecb.SetComponent(newChampion, new GhostOwner { NetworkId = clientId });
                ecb.SetComponent(newChampion, new GameTeam { Value = requestedTeamType });

                ecb.AppendToBuffer(requestSource.ValueRO.SourceConnection, new LinkedEntityGroup { Value = newChampion });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}