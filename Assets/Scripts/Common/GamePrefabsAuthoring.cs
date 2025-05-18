using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common
{
    public class GamePrefabsAuthoring : MonoBehaviour
    {
        public GameObject Champion;

        public class GamePrefabsBaker : Baker<GamePrefabsAuthoring>
        {
            public override void Bake(GamePrefabsAuthoring authoring)
            {
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabContainerEntity, new GamePrefabs
                {
                    Champion = GetEntity(authoring.Champion, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}