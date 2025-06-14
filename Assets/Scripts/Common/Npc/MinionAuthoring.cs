﻿using ECS_Multiplayer.Common.Champion;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace ECS_Multiplayer.Common.Npc
{
    public class MinionAuthoring : MonoBehaviour
    {
        public float MoveSpeed;
        
        public class MinionBaker : Baker<MinionAuthoring>
        {
            public override void Bake(MinionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<MinionTag>(entity);
                AddComponent<NewMinionTag>(entity);
                AddComponent(entity, new CharacterMoveSpeed { Value = authoring.MoveSpeed });
                AddComponent<MinionPathIndex>(entity);
                AddBuffer<MinionPathPosition>(entity);
                AddComponent<GameTeam>(entity);
                AddComponent<URPMaterialPropertyBaseColor>(entity);
            }
        }
    }
}