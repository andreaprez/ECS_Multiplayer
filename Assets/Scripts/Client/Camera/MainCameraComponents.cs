using Unity.Entities;

namespace ECS_Multiplayer.Client.Camera
{
    public class MainCamera : IComponentData
    {
        public UnityEngine.Camera Value;
    }

    public struct MainCameraTag : IComponentData {}
}