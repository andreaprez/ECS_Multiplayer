using ECS_Multiplayer.Common.Champion;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace ECS_Multiplayer.Client.Champion
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]  
    public partial struct InitializeLocalChampionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transform, entity) in 
                     SystemAPI.Query<RefRO<LocalTransform>>().WithAll<GhostOwnerIsLocal>().WithNone<OwnerChampionTag>().WithEntityAccess())
            {
                ecb.AddComponent<OwnerChampionTag>(entity);
                ecb.SetComponent(entity, new ChampionMoveTargetPosition { Value = transform.ValueRO.Position });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}