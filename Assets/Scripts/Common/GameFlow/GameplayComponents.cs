using Unity.Entities;
using Unity.NetCode;

namespace ECS_Multiplayer.Common
{
    public struct GameStartTick : IComponentData
    {
        public NetworkTick Value;
    }

    public struct GamePlayingTag : IComponentData {}

    public struct GameOverTag : IComponentData {}

    public struct WinningTeam : IComponentData
    {
        [GhostField] public TeamType Value;
    }
    
    public struct GameOverOnDestroyTag : IComponentData {}
}