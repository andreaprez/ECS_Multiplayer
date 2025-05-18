using ECS_Multiplayer.Common;
using Unity.Entities;

namespace ECS_Multiplayer.Client
{
    public struct ClientTeamRequest : IComponentData
    {
        public TeamType Value;
    }
}