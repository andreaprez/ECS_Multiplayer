using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace ECS_Multiplayer.Common.Champion
{
    public struct ChampionTag : IComponentData { }
    
    public struct NewChampionTag : IComponentData { }
    
    public struct OwnerChampionTag : IComponentData { }
    
    public struct GameTeam : IComponentData
    {
        [GhostField] public TeamType Value;
    }
    
    public struct CharacterMoveSpeed : IComponentData
    { 
        public float Value;
    }
    
    public struct ChampionMoveTargetPosition : IInputComponentData
    { 
        [GhostField(Quantization = 0)] public float3 Value;
    }
    
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AbilityInput : IInputComponentData
    { 
        [GhostField] public InputEvent AoeAbility;
    }
}