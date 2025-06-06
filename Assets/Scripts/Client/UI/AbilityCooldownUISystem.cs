using ECS_Multiplayer.Common.Combat;
using Unity.Entities;
using Unity.NetCode;

namespace ECS_Multiplayer.Client.UI
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct AbilityCooldownUISystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var abilityCooldownUIController = AbilityCooldownUIController.Instance;

            foreach (var (cooldownTargetTicks, abilityCooldownTicks) in 
                     SystemAPI.Query<DynamicBuffer<AbilityCooldownTargetTicks>, RefRO<AbilityCooldownTicks>>())
            {
                if (!cooldownTargetTicks.GetDataAtTick(currentTick, out var currentTargetTicks))
                {
                    currentTargetTicks.AoeAbility = NetworkTick.Invalid;
                    currentTargetTicks.SkillShotAbility = NetworkTick.Invalid;
                }

                if (currentTargetTicks.AoeAbility == NetworkTick.Invalid ||
                    currentTick.IsNewerThan(currentTargetTicks.AoeAbility))
                {
                    abilityCooldownUIController.UpdateAoeCooldownMask(0f);
                }
                else
                {
                    var aoeRemainingTickCount = currentTargetTicks.AoeAbility.TickIndexForValidTick -
                                                currentTick.TickIndexForValidTick;
                    var aoeCooldownFillAmount = (float)aoeRemainingTickCount / abilityCooldownTicks.ValueRO.AoeAbility;
                    abilityCooldownUIController.UpdateAoeCooldownMask(aoeCooldownFillAmount);
                }
                
                if (currentTargetTicks.SkillShotAbility == NetworkTick.Invalid ||
                    currentTick.IsNewerThan(currentTargetTicks.SkillShotAbility))
                {
                    abilityCooldownUIController.UpdateSkillShotCooldownMask(0f);
                }
                else
                {
                    var skillShotRemainingTickCount = currentTargetTicks.SkillShotAbility.TickIndexForValidTick -
                                                currentTick.TickIndexForValidTick;
                    var skillShotCooldownFillAmount = (float)skillShotRemainingTickCount / abilityCooldownTicks.ValueRO.SkillShotAbility;
                    abilityCooldownUIController.UpdateSkillShotCooldownMask(skillShotCooldownFillAmount);
                }
            }
        }
    }
}