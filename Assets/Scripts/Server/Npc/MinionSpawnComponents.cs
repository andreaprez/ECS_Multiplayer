using Unity.Entities;

namespace ECS_Multiplayer.Server.Npc
{
    public struct MinionSpawnProperties : IComponentData
    {
        public float TimeBetweenWaves;
        public float TimeBetweenMinions;
        public int CountToSpawnInWave;
    }

    public struct MinionSpawnTimers : IComponentData
    {
        public float TimeToNextWave;
        public float TimeToNextMinion;
        public int CountSpawnedInWave;
    }
    
    public struct MinionPathContainers : IComponentData
    {
        public Entity TopLane;
        public Entity MiddleLane;
        public Entity BottomLane;
    }
}