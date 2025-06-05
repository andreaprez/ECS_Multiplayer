using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common.Combat
{
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject AoeAbilityPrefab;
        
        public class AbilityBaker : Baker<AbilityAuthoring>
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = GetEntity(authoring.AoeAbilityPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}