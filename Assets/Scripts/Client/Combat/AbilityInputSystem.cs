using ECS_Multiplayer.Common;
using ECS_Multiplayer.Common.Champion;
using Unity.Entities;

namespace ECS_Multiplayer.Client.Combat
{
    public partial class AbilityInputSystem : SystemBase
    {
        private GameInputActions _inputActions;

        protected override void OnCreate()
        {
            RequireForUpdate<GamePlayingTag>();
            _inputActions = new GameInputActions();
        }

        protected override void OnStartRunning()
        {
            _inputActions.Enable();
        }

        protected override void OnStopRunning()
        {
            _inputActions.Disable();
        }

        protected override void OnUpdate()
        {
            var newAbilityInput = new AbilityInput();

            if (_inputActions.GameplayMap.AoeAbility.WasPressedThisFrame())
            {
                newAbilityInput.AoeAbility.Set();
            }

            if (_inputActions.GameplayMap.SkillShotAbility.WasPressedThisFrame())
            {
                newAbilityInput.SkillShotAbility.Set();
            }
            
            if (_inputActions.GameplayMap.ConfirmSkillShotAbility.WasPressedThisFrame())
            {
                newAbilityInput.ConfirmSkillShotAbility.Set();
            }
            
            foreach (var abilityInput in SystemAPI.Query<RefRW<AbilityInput>>())
            {
                abilityInput.ValueRW = newAbilityInput;
            }
        }
    }
}