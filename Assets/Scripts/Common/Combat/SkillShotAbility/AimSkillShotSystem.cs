using ECS_Multiplayer.Client.Camera;
using ECS_Multiplayer.Common.Champion;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace ECS_Multiplayer.Common.Combat
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct AimSkillShotSystem : ISystem
    {
        private CollisionFilter _selectionFilter;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainCameraTag>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            
            _selectionFilter = new CollisionFilter
            {
                BelongsTo = 1 << 5,
                CollidesWith = 1 << 0
            };
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (aimInput, transform) in SystemAPI.Query<RefRW<AimInput>, RefRW<LocalTransform>>().WithAll<AimSkillShotTag, OwnerChampionTag>())
            {
                var cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
                var mainCamera = state.EntityManager.GetComponentObject<MainCamera>(cameraEntity).Value;

                var mousePosition = Input.mousePosition;
                mousePosition.z = 100f;
                var mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);

                var selectionInput = new RaycastInput
                {
                    Start = mainCamera.transform.position,
                    End = mousePositionInWorld,
                    Filter = _selectionFilter
                };
            
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                if (collisionWorld.CastRay(selectionInput, out var closestHit))
                {
                    var directionToTarget = closestHit.Position - transform.ValueRO.Position;
                    directionToTarget.y = transform.ValueRO.Position.y;
                    directionToTarget = math.normalize(directionToTarget);
                    aimInput.ValueRW.Value = directionToTarget;
                }
            }
        }
    }
}