using Unity.NetCode;

namespace ECS_Multiplayer.Client.Common
{
    public struct TeamRequest : IRpcCommand
    {
        public TeamType Value;
    }
}