﻿using ECS_Multiplayer.Common.Champion;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS_Multiplayer.Common.Combat
{
    public readonly partial struct SkillShotAspect : IAspect
    {
        public readonly Entity ChampionEntity;
        
        private readonly RefRO<AbilityInput> _abilityInput;
        private readonly RefRO<AbilityPrefabs> _abilityPrefabs;
        private readonly RefRO<AbilityCooldownTicks> _abilityCooldownTicks;
        private readonly DynamicBuffer<AbilityCooldownTargetTicks> _abilityCooldownTargetTicks;
        private readonly RefRO<AimInput> _aimInput;
        private readonly RefRO<GameTeam> _team;
        private readonly RefRO<LocalTransform> _localTransform;

        public bool ShouldAttack => _abilityInput.ValueRO.SkillShotAbility.IsSet;
        public bool ConfirmAttack => _abilityInput.ValueRO.ConfirmSkillShotAbility.IsSet;
        public float3 AttackPosition => _localTransform.ValueRO.Position;
        public LocalTransform SpawnTransform => LocalTransform.FromPositionRotation(
            _localTransform.ValueRO.Position + new float3(0, 2, 0), 
            quaternion.LookRotationSafe(_aimInput.ValueRO.Value, math.up()));
        public Entity AbilityPrefab => _abilityPrefabs.ValueRO.SkillShotAbility;
        public uint CooldownTicks => _abilityCooldownTicks.ValueRO.SkillShotAbility;
        public DynamicBuffer<AbilityCooldownTargetTicks> CooldownTargetTicks => _abilityCooldownTargetTicks;
        public GameTeam Team => _team.ValueRO;
    }
}