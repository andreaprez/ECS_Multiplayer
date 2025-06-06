using UnityEngine;

namespace ECS_Multiplayer.Client.UI
{
    public class HealthBarRotation : MonoBehaviour
    {
        private UnityEngine.Camera _camera;
        
        private void Start()
        {
            _camera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            if (_camera == null)
                return;
            
            var cameraPosition = _camera.transform.position;
            var targetPos = new Vector3(transform.position.x, cameraPosition.y, cameraPosition.z);
            transform.LookAt(targetPos);
        }
    }
}