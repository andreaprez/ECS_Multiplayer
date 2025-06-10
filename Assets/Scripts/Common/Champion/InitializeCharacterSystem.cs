using ECS_Multiplayer.Common.Npc;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

namespace ECS_Multiplayer.Common.Champion
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct InitializeCharacterSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (physicsMass, gameTeam, newCharacterEntity) in 
                     SystemAPI.Query<RefRW<PhysicsMass>, RefRO<GameTeam>>().WithAny<NewChampionTag, NewMinionTag>().WithEntityAccess())
            {
                // Setting it as kinematic:
                physicsMass.ValueRW.InverseInertia[0] = 0;
                physicsMass.ValueRW.InverseInertia[1] = 0;
                physicsMass.ValueRW.InverseInertia[2] = 0;

                var teamColor = gameTeam.ValueRO.Value switch
                {
                    TeamType.Blue => new float4(0.03f, 0.07f, 0.9f, 1),
                    TeamType.Red => new float4(0.8f, 0.1f, 0.1f, 1),
                    _ => new float4()
                };
                ecb.SetComponent(newCharacterEntity, new URPMaterialPropertyBaseColor { Value = teamColor });

                ecb.RemoveComponent<NewChampionTag>(newCharacterEntity);
                ecb.RemoveComponent<NewMinionTag>(newCharacterEntity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}