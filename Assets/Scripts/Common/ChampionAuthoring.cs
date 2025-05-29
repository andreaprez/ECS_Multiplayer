using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Common
{
    public class ChampionAuthoring : MonoBehaviour
    {
        public class ChampionBaker : Baker<ChampionAuthoring>
        {
            public override void Bake(ChampionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ChampionTag>(entity);
                AddComponent<NewChampionTag>(entity);
                AddComponent<GameTeam>(entity);
            }
        }
    }
}