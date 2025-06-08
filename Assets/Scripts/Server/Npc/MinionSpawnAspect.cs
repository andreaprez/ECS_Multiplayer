using Unity.Entities;

namespace ECS_Multiplayer.Server.Npc
{
    public readonly partial struct MinionSpawnAspect : IAspect
    {
        private readonly RefRW<MinionSpawnTimers> _minionSpawnTimers;
        private readonly RefRO<MinionSpawnProperties> _minionSpawnProperties;
        
        public int CountSpawnedInWave
        {
            get => _minionSpawnTimers.ValueRO.CountSpawnedInWave;
            set => _minionSpawnTimers.ValueRW.CountSpawnedInWave = value;
        }
        
        private float TimeToNextWave
        {
            get => _minionSpawnTimers.ValueRO.TimeToNextWave;
            set => _minionSpawnTimers.ValueRW.TimeToNextWave = value;
        }
        
        private float TimeToNextMinion
        {
            get => _minionSpawnTimers.ValueRO.TimeToNextMinion;
            set => _minionSpawnTimers.ValueRW.TimeToNextMinion = value;
        }

        private int CountToSpawnInWave => _minionSpawnProperties.ValueRO.CountToSpawnInWave;
        private float TimeBetweenWaves => _minionSpawnProperties.ValueRO.TimeBetweenWaves;
        private float TimeBetweenMinions => _minionSpawnProperties.ValueRO.TimeBetweenMinions;

        public bool ShouldSpawn => TimeToNextWave <= 0f && TimeToNextMinion <= 0f;
        public bool IsWaveSpawned => CountSpawnedInWave >= CountToSpawnInWave;

        public void DecrementTimers(float deltaTime)
        {
            if (TimeToNextWave >= 0f)
            {
                TimeToNextWave -= deltaTime;
                return;
            }

            if (TimeToNextMinion >= 0f)
            {
                TimeToNextMinion -= deltaTime;
            }
        }

        public void ResetSpawnCounter()
        {
            CountSpawnedInWave = 0;
        }
        
        public void ResetWaveTimer()
        {
            TimeToNextWave = TimeBetweenWaves;
        }
        
        public void ResetMinionTimer()
        {
            TimeToNextMinion = TimeBetweenMinions;
        }
    }
}