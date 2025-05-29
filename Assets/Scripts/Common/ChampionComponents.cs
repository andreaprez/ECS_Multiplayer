using Unity.Entities;
using Unity.NetCode;

namespace ECS_Multiplayer.Common
{
    public struct ChampionTag : IComponentData { }
    public struct NewChampionTag : IComponentData { }
    public struct GameTeam : IComponentData
    {
        [GhostField] public TeamType Value;
    }
}