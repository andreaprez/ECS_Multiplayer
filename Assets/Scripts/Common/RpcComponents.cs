using Unity.NetCode;

namespace ECS_Multiplayer.Common
{
    public struct TeamRequest : IRpcCommand
    {
        public TeamType Value;
    }
}