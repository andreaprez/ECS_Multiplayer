﻿using ECS_Multiplayer.Common.Champion;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS_Multiplayer.Common.Combat
{
    public readonly partial struct AoeAspect : IAspect
    {
        private readonly RefRO<AbilityInput> _abilityInput;
        private readonly RefRO<AbilityPrefabs> _abilityPrefabs;
        private readonly RefRO<GameTeam> _team;
        private readonly RefRO<LocalTransform> _localTransform;
        private readonly RefRO<AbilityCooldownTicks> _abilityCooldownTicks;
        private readonly DynamicBuffer<AbilityCooldownTargetTicks> _abilityCooldownTargetTicks;

        public bool ShouldAttack => _abilityInput.ValueRO.AoeAbility.IsSet;
        public Entity AbilityPrefab => _abilityPrefabs.ValueRO.AoeAbility;
        public GameTeam Team => _team.ValueRO;
        public float3 AttackPosition => _localTransform.ValueRO.Position;
        public uint CooldownTicks => _abilityCooldownTicks.ValueRO.AoeAbility;
        public DynamicBuffer<AbilityCooldownTargetTicks> CooldownTargetTicks => _abilityCooldownTargetTicks;
    }
}