using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace ECS_Multiplayer.Common.Combat
{
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject AoeAbilityPrefab;
        public float AoeAbilityCooldownSeconds;
        public NetCodeConfig NetCodeConfig;
        
        private int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        public class AbilityBaker : Baker<AbilityAuthoring>
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = GetEntity(authoring.AoeAbilityPrefab, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity, new AbilityCooldownTicks()
                {
                    AoeAbility = (uint)(authoring.AoeAbilityCooldownSeconds * authoring.SimulationTickRate)
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}