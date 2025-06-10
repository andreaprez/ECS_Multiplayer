using ECS_Multiplayer.Common;
using ECS_Multiplayer.Common.Champion;
using ECS_Multiplayer.Common.Respawn;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace ECS_Multiplayer.Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntryRequestSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameStartProperties>();
            state.RequireForUpdate<GamePrefabs>();
            state.RequireForUpdate<NetworkTime>();
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<TeamRequestRpc, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var championPrefab = SystemAPI.GetSingleton<GamePrefabs>().Champion;

            var gameStartPropertiesEntity = SystemAPI.GetSingletonEntity<GameStartProperties>();
            var gameStartProperties = SystemAPI.GetComponent<GameStartProperties>(gameStartPropertiesEntity);
            var teamPlayerCounter = SystemAPI.GetComponent<TeamPlayerCounter>(gameStartPropertiesEntity);
            var spawnOffsets = SystemAPI.GetBuffer<SpawnOffset>(gameStartPropertiesEntity);
            
            foreach (var (teamRequest, requestSource, requestEntity) in 
                     SystemAPI.Query<RefRO<TeamRequestRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                ecb.DestroyEntity(requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.ValueRO.SourceConnection);

                var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.ValueRO.SourceConnection).Value;
                
                var requestedTeamType = teamRequest.ValueRO.Value;
                if (requestedTeamType == TeamType.AutoAssign)
                {
                    if (teamPlayerCounter.BlueTeamPlayers > teamPlayerCounter.RedTeamPlayers)
                    {
                        requestedTeamType = TeamType.Red;
                    }
                    else if (teamPlayerCounter.BlueTeamPlayers <= teamPlayerCounter.RedTeamPlayers)
                    {
                        requestedTeamType = TeamType.Blue;
                    }
                }

                float3 spawnPosition;
                switch (requestedTeamType)
                {
                    case TeamType.Blue:
                        if (teamPlayerCounter.BlueTeamPlayers >= gameStartProperties.MaxPlayersPerTeam)
                            continue;
                        spawnPosition = new float3(-75f, 0f, -75f);
                        spawnPosition += spawnOffsets[teamPlayerCounter.BlueTeamPlayers].Value;
                        teamPlayerCounter.BlueTeamPlayers++;
                        break;
                    
                    case TeamType.Red:
                        if (teamPlayerCounter.RedTeamPlayers >= gameStartProperties.MaxPlayersPerTeam)
                            continue;
                        spawnPosition = new float3(75f, 0f, 75f);
                        spawnPosition += spawnOffsets[teamPlayerCounter.RedTeamPlayers].Value;
                        teamPlayerCounter.RedTeamPlayers++;
                        break;
                    
                    default:
                        continue;
                }

                var newChampion = ecb.Instantiate(championPrefab);
                ecb.SetName(newChampion, "Champion");
                var newTransform = LocalTransform.FromPosition(spawnPosition);
                ecb.SetComponent(newChampion, newTransform);
                ecb.SetComponent(newChampion, new GhostOwner { NetworkId = clientId });
                ecb.SetComponent(newChampion, new GameTeam { Value = requestedTeamType });

                ecb.AppendToBuffer(requestSource.ValueRO.SourceConnection, new LinkedEntityGroup { Value = newChampion });

                ecb.SetComponent(newChampion, new NetworkEntityReference
                {
                    Value = requestSource.ValueRO.SourceConnection
                });
                ecb.AddComponent(requestSource.ValueRO.SourceConnection, new PlayerSpawnInfo
                {
                    Team = requestedTeamType,
                    SpawnPosition = spawnPosition
                });

                var gameStartRpc = ecb.CreateEntity();
                var playersRemainingToStart = gameStartProperties.MinPlayersToStartGame - teamPlayerCounter.TotalPlayers;
                if (playersRemainingToStart <= 0 && !SystemAPI.HasSingleton<GamePlayingTag>())
                {
                    var simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
                    var ticksUntilStart = (uint)(simulationTickRate * gameStartProperties.CountdownTime);
                    var gameStartTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
                    gameStartTick.Add(ticksUntilStart);
                    ecb.AddComponent(gameStartRpc, new GameStartTickRpc
                    {
                        Value = gameStartTick
                    });
                    
                    var gameStartEntity = ecb.CreateEntity();
                    ecb.AddComponent(gameStartEntity, new GameStartTick
                    {
                        Value = gameStartTick
                    });
                }
                else
                {
                    ecb.AddComponent(gameStartRpc, new PlayersRemainingToStartRpc { Value = playersRemainingToStart });
                }
                ecb.AddComponent<SendRpcCommandRequest>(gameStartRpc);
            }

            ecb.Playback(state.EntityManager);
            SystemAPI.SetSingleton(teamPlayerCounter);
        }
    }
}