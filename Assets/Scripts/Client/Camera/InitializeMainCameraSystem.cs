using Unity.Entities;

namespace ECS_Multiplayer.Client.Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class InitializeMainCameraSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<MainCameraTag>();
        }
        
        protected override void OnUpdate()
        {
            Enabled = false;
            var mainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            EntityManager.SetComponentData(mainCameraEntity, new MainCamera { Value = UnityEngine.Camera.main });
        }
    }
}