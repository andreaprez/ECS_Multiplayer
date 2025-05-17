using ECS_Multiplayer.Client.Common;
using Unity.Entities;

namespace ECS_Multiplayer.Client
{
    public struct ClientTeamRequest : IComponentData
    {
        public TeamType Value;
    }
}