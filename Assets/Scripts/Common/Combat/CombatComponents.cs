﻿using Unity.Entities;
using Unity.NetCode;

namespace ECS_Multiplayer.Common.Combat
{
    public struct MaxHitPoints : IComponentData
    {
        public int Value;
    }
    
    public struct CurrentHitPoints : IComponentData
    {
        [GhostField] public int Value;
    }
    
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct DamageBuffer : IBufferElementData
    {
        public int Value;
    }
    
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted, OwnerSendType = SendToOwnerType.SendToNonOwner)]
    public struct DamageThisTick : ICommandData
    {
        public NetworkTick Tick { get; set; }
        public int Value;
    }

    public struct AbilityPrefabs : IComponentData
    {
        public Entity AoeAbility;
        public Entity SkillShotAbility;
    }
    
    public struct DestroyOnTimer : IComponentData
    {
        public float Value;
    }
    
    public struct DestroyAtTick : IComponentData
    {
        [GhostField] public NetworkTick Value;
    }
    
    public struct DestroyEntityTag : IComponentData { }

    public struct DamageOnTrigger : IComponentData
    {
        public int Value;
    }

    public struct AlreadyDamagedEntityBuffer : IBufferElementData
    {
        public Entity Value;
    }
    
    public struct AbilityCooldownTicks : IComponentData
    {
        public uint AoeAbility;
        public uint SkillShotAbility;
    }
    
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AbilityCooldownTargetTicks : ICommandData
    {
        public NetworkTick Tick { get; set; }
        public NetworkTick AoeAbility;
        public NetworkTick SkillShotAbility;
    }
    
    public struct AimSkillShotTag : IComponentData { }

    public struct AbilityMoveSpeed : IComponentData
    {
        public float Value;
    }
}