using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace ECS_Multiplayer.Common.Champion
{
    public class ChampionAuthoring : MonoBehaviour
    {
        public float MoveSpeed;
        
        public class ChampionBaker : Baker<ChampionAuthoring>
        {
            public override void Bake(ChampionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ChampionTag>(entity);
                AddComponent<NewChampionTag>(entity);
                AddComponent<GameTeam>(entity);
                AddComponent<URPMaterialPropertyBaseColor>(entity);
                AddComponent<ChampionMoveTargetPosition>(entity);
                AddComponent(entity, new CharacterMoveSpeed { Value = authoring.MoveSpeed });
                AddComponent<AbilityInput>(entity);
                AddComponent<AimInput>(entity);
            }
        }
    }
}