using Unity.NetCode;

namespace ECS_Multiplayer.Common
{
    public struct TeamRequestRpc : IRpcCommand
    {
        public TeamType Value;
    }
    
    public struct PlayersRemainingToStartRpc : IRpcCommand
    {
        public int Value;
    }
    
    public struct GameStartTickRpc : IRpcCommand
    {
        public NetworkTick Value;
    }
}