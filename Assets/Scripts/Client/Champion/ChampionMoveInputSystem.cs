using ECS_Multiplayer.Client.Camera;
using ECS_Multiplayer.Common.Champion;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECS_Multiplayer.Client.Champion
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class ChampionMoveInputSystem : SystemBase
    {
        private GameInputActions _inputActions;
        private CollisionFilter _selectionFilter;

        protected override void OnCreate()
        {
            _inputActions = new GameInputActions();
            _selectionFilter = new CollisionFilter
            {
                BelongsTo = 1 << 5,
                CollidesWith = 1 << 0
            };
            RequireForUpdate<OwnerChampionTag>();
        }

        protected override void OnStartRunning()
        {
            _inputActions.Enable();
            _inputActions.GameplayMap.SelectMovePosition.performed += OnSelectMovePosition;
        }

        protected override void OnStopRunning()
        {            
            _inputActions.GameplayMap.SelectMovePosition.performed -= OnSelectMovePosition;
            _inputActions.Disable();
        }

        private void OnSelectMovePosition(InputAction.CallbackContext obj)
        {
            var cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            var mainCamera = EntityManager.GetComponentObject<MainCamera>(cameraEntity).Value;

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
                var championEntity = SystemAPI.GetSingletonEntity<OwnerChampionTag>();
                EntityManager.SetComponentData(championEntity, new ChampionMoveTargetPosition
                {
                    Value = closestHit.Position
                });
            }
        }

        protected override void OnUpdate() { }
    }
}