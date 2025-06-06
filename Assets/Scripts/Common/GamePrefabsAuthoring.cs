using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common
{
    public class GamePrefabsAuthoring : MonoBehaviour
    {
        [Header("Entities")]
        public GameObject Champion;
        
        [Header("GameObjects")]
        public GameObject HealthBarPrefab;

        public class GamePrefabsBaker : Baker<GamePrefabsAuthoring>
        {
            public override void Bake(GamePrefabsAuthoring authoring)
            {
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabContainerEntity, new GamePrefabs
                {
                    Champion = GetEntity(authoring.Champion, TransformUsageFlags.Dynamic)
                });
                
                AddComponentObject(prefabContainerEntity, new UIPrefabs
                {
                    HealthBar = authoring.HealthBarPrefab
                });
            }
        }
    }
}