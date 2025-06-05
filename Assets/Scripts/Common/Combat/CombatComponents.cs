using Unity.Entities;
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
}