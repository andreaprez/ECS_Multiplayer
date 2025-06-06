using ECS_Multiplayer.Common.Npc;
using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Server.Npc
{
    public class MinionPathAuthoring : MonoBehaviour
    {
        public Vector3[] TopLanePath;
        public Vector3[] MiddleLanePath;
        public Vector3[] BottomLanePath;
        
        public class MinionPathBaker : Baker<MinionPathAuthoring>
        {
            public override void Bake(MinionPathAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var topLane = CreateAdditionalEntity(TransformUsageFlags.None, false, "TopLane");
                var middleLane = CreateAdditionalEntity(TransformUsageFlags.None, false, "MiddleLane");
                var bottomLane = CreateAdditionalEntity(TransformUsageFlags.None, false, "BottomLane");

                var topLanePath = AddBuffer<MinionPathPosition>(topLane);
                foreach (var pathPosition in authoring.TopLanePath)
                {
                    topLanePath.Add(new MinionPathPosition { Value = pathPosition });
                }
                
                var middleLanePath = AddBuffer<MinionPathPosition>(middleLane);
                foreach (var pathPosition in authoring.MiddleLanePath)
                {
                    middleLanePath.Add(new MinionPathPosition { Value = pathPosition });
                }
                
                var bottomLanePath = AddBuffer<MinionPathPosition>(bottomLane);
                foreach (var pathPosition in authoring.BottomLanePath)
                {
                    bottomLanePath.Add(new MinionPathPosition { Value = pathPosition });
                }

                AddComponent(entity, new MinionPathContainers
                {
                    TopLane = topLane,
                    MiddleLane = middleLane,
                    BottomLane = bottomLane
                });
            }
        }
    }
}