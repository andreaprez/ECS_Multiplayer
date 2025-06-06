using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace ECS_Multiplayer.Common.Combat
{
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject AoeAbilityPrefab;
        public GameObject SkillShotAbilityPrefab;
        
        public float AoeAbilityCooldownSeconds;
        public float SkillshotAbilityCooldownSeconds;
        
        public NetCodeConfig NetCodeConfig;
        
        private int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        public class AbilityBaker : Baker<AbilityAuthoring>
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = GetEntity(authoring.AoeAbilityPrefab, TransformUsageFlags.Dynamic),
                    SkillShotAbility = GetEntity(authoring.SkillShotAbilityPrefab, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity, new AbilityCooldownTicks()
                {
                    AoeAbility = (uint)(authoring.AoeAbilityCooldownSeconds * authoring.SimulationTickRate),
                    SkillShotAbility = (uint)(authoring.SkillshotAbilityCooldownSeconds * authoring.SimulationTickRate)
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}