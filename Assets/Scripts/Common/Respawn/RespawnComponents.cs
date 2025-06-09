using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace ECS_Multiplayer.Common.Respawn
{
    public struct RespawnEntityTag : IComponentData {}

    public struct RespawnBuffer : IBufferElementData
    {
        [GhostField] public NetworkTick RespawnTick;
        [GhostField] public Entity NetworkEntity;
        [GhostField] public int NetworkId;
    }

    public struct RespawnTickCount : IComponentData
    {
        public uint Value;
    }
    
    public struct PlayerSpawnInfo : IComponentData
    {
        public TeamType Team;
        public float3 SpawnPosition;
    }
    
    public struct NetworkEntityReference : IComponentData
    {
        public Entity Value;
    }
}