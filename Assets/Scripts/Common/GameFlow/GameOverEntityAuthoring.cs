using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common
{
    public class GameOverEntityAuthoring : MonoBehaviour
    {
        public class GameOverEntityBaker : Baker<GameOverEntityAuthoring>
        {
            public override void Bake(GameOverEntityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<GameOverTag>(entity);
                AddComponent<WinningTeam>(entity);
            }
        }
    }
}