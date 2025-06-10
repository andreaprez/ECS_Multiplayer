using System;
using ECS_Multiplayer.Common;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace ECS_Multiplayer.Client
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class ClientStartGameSystem : SystemBase
    {
        public Action<int> OnUpdatePlayersRemainingToStart;
        public Action OnStartGameCountdown;
        public Action OnSkipWait;

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (playersRemainingToStart, entity) in 
                     SystemAPI.Query<RefRO<PlayersRemainingToStartRpc>>().WithAll<ReceiveRpcCommandRequest>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                OnUpdatePlayersRemainingToStart?.Invoke(playersRemainingToStart.ValueRO.Value);
            }
            
            foreach (var (gameStartTick, entity) in 
                     SystemAPI.Query<RefRO<GameStartTickRpc>>().WithAll<ReceiveRpcCommandRequest>().WithNone<GamePlayingTag>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);

                var gameStartEntity = ecb.CreateEntity();
                ecb.AddComponent(gameStartEntity, new GameStartTick
                {
                    Value = gameStartTick.ValueRO.Value
                });

                var networkTime = SystemAPI.GetSingleton<NetworkTime>();
                if (gameStartTick.ValueRO.Value.TickIndexForValidTick <= networkTime.ServerTick.TickIndexForValidTick)
                    OnSkipWait?.Invoke();
                else
                    OnStartGameCountdown?.Invoke();
            }
            
            ecb.Playback(EntityManager);
        }
    }
}