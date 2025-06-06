using ECS_Multiplayer.Common.Champion;
using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common
{
    public class GameTeamAuthoring : MonoBehaviour
    {
        public TeamType Team;
        
        public class GameTeamBaker : Baker<GameTeamAuthoring>
        {
            public override void Bake(GameTeamAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new GameTeam { Value = authoring.Team });
            }
        }
    }
}