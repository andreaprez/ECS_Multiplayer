using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common.Combat
{
    public class HitPointsAuthoring : MonoBehaviour
    {
        public int MaxHitPoints;
        
        public class HitPointsBaker : Baker<HitPointsAuthoring>
        {
            public override void Bake(HitPointsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CurrentHitPoints { Value = authoring.MaxHitPoints });
                AddComponent(entity, new MaxHitPoints() { Value = authoring.MaxHitPoints });
                AddBuffer<DamageBuffer>(entity);
                AddBuffer<DamageThisTick>(entity);
            }
        }
    }
}