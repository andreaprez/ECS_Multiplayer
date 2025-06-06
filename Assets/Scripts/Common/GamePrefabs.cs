using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common
{
    public struct GamePrefabs : IComponentData
    {
        public Entity Champion;
    }
    
    public class UIPrefabs : IComponentData
    {
        public GameObject HealthBar;
    }
}