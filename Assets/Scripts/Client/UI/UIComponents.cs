using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS_Multiplayer.Client.UI
{
    public class HealthBarUIReference : ICleanupComponentData
    {
        public GameObject Value;
    }
    
    public struct HealthBarOffset : IComponentData
    {
        public float3 Value;
    }
}