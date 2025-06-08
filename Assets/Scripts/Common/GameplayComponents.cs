using Unity.Entities;
using Unity.NetCode;

namespace ECS_Multiplayer.Common
{
    public struct GamePlayingTag : IComponentData {}

    public struct GameStartTick : IComponentData
    {
        public NetworkTick Value;
    }

}