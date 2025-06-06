using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common.Combat
{
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        public int DamageOnTrigger;
        
        public class DamageOnTriggerBaker : Baker<DamageOnTriggerAuthoring>
        {
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageOnTrigger { Value = authoring.DamageOnTrigger });
                AddBuffer<AlreadyDamagedEntityBuffer>(entity);
            }
        }
    }
}