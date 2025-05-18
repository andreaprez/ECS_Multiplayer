using Cinemachine;
using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Client.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
        
        [Header("Move Settings")]
        [SerializeField] private bool _drawBounds;
        [SerializeField] private Bounds _cameraBounds;
        [SerializeField] private float _camSpeed;
        [SerializeField] private Vector2 _screenPercentageDetection;

        [Header("Zoom Settings")]
        [SerializeField] private float _minZoomDistance;
        [SerializeField] private float _maxZoomDistance;
        [SerializeField] private float _zoomSpeed;

        [Header("Camera Start Positions")] 
        [SerializeField] private Vector3 _spectatorPosition = new(0f, 0f, 0f);
        
        private Vector2 _normalScreenPercentage;
        private Vector2 NormalMousePos => new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        private bool InScreenLeft => NormalMousePos.x < _normalScreenPercentage.x  && Application.isFocused;
        private bool InScreenRight => NormalMousePos.x > 1 - _normalScreenPercentage.x  && Application.isFocused;
        private bool InScreenTop => NormalMousePos.y < _normalScreenPercentage.y  && Application.isFocused;
        private bool InScreenBottom => NormalMousePos.y > 1 - _normalScreenPercentage.y  && Application.isFocused;

        private CinemachineFramingTransposer _transposer;
        private EntityManager _entityManager;
        private EntityQuery _teamControllerQuery;
        private EntityQuery _localChampQuery;
        private bool _cameraSet;
        
        private void Awake()
        {
            _normalScreenPercentage = _screenPercentageDetection * 0.01f;
            _transposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        private void Start()
        {
            transform.position = _spectatorPosition;
        }

        private void OnValidate()
        {
            _normalScreenPercentage = _screenPercentageDetection * 0.01f;
        }

        private void Update()
        {
            MoveCamera();
            ZoomCamera();
        }

        private void MoveCamera()
        {
            if (InScreenLeft)
            {
                transform.position += Vector3.left * (_camSpeed * Time.deltaTime);
            }

            if (InScreenRight)
            {
                transform.position += Vector3.right * (_camSpeed * Time.deltaTime);
            }

            if (InScreenTop)
            {
                transform.position += Vector3.back * (_camSpeed * Time.deltaTime);
            }

            if (InScreenBottom)
            {
                transform.position += Vector3.forward * (_camSpeed * Time.deltaTime);
            }
            
            if (!_cameraBounds.Contains(transform.position))
            {
                transform.position = _cameraBounds.ClosestPoint(transform.position);
            }
        }

        private void ZoomCamera()
        {
            if (Mathf.Abs(Input.mouseScrollDelta.y) > float.Epsilon)
            {
                _transposer.m_CameraDistance -= Input.mouseScrollDelta.y * _zoomSpeed * Time.deltaTime;
                _transposer.m_CameraDistance =
                    Mathf.Clamp(_transposer.m_CameraDistance, _minZoomDistance, _maxZoomDistance);
            }
        }

        private void OnDrawGizmos()
        {
            if (!_drawBounds) return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_cameraBounds.center, _cameraBounds.size);
        }
    }
}