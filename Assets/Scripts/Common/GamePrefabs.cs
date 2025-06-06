using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common
{
    public struct GamePrefabs : IComponentData
    {
        public Entity Champion;
        public Entity Minion;
    }
    
    public class UIPrefabs : IComponentData
    {
        public GameObject HealthBar;
        public GameObject SkillShotAim;
    }
}