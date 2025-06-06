using Cinemachine;
using ECS_Multiplayer.Common;
using ECS_Multiplayer.Common.Champion;
using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Client.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        
        [Header("Movement Settings")]
        [SerializeField] private Vector2 screenPercentageDetection;
        [SerializeField] private Bounds cameraBounds;
        [SerializeField] private float camSpeed;

        [Header("Zoom Settings")]
        [SerializeField] private float minZoomDistance;
        [SerializeField] private float maxZoomDistance;
        [SerializeField] private float zoomSpeed;

        [Header("Start Positions")] 
        [SerializeField] private Vector3 blueTeamPosition = new(-70f, 0f, -70f);
        [SerializeField] private Vector3 redTeamPosition = new(70f, 0f, 70f);
        [SerializeField] private Vector3 spectatorPosition = new(0f, 0f, 0f);
        
        private Vector2 _normalScreenPercentage;
        private Vector2 NormalMousePos => new (Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
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
            _normalScreenPercentage = screenPercentageDetection * 0.01f;
            _transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        private void Start()
        {
            if (World.DefaultGameObjectInjectionWorld == null)
                return;
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _teamControllerQuery = _entityManager.CreateEntityQuery(typeof(ClientTeamRequest));
            _localChampQuery = _entityManager.CreateEntityQuery(typeof(OwnerChampionTag));

            SetInitialCameraPosition();
        }

        private void Update()
        {
            SetCameraForAutoAssignedTeam();
            MoveCamera();
            ZoomCamera();
        }

        private void SetInitialCameraPosition()
        {
            if (_teamControllerQuery.TryGetSingleton<ClientTeamRequest>(out var requestedTeam))
            {
                var team = requestedTeam.Value;
                var cameraPosition = team switch
                {
                    TeamType.Blue => blueTeamPosition,
                    TeamType.Red => redTeamPosition,
                    _ => spectatorPosition
                };
                transform.position = cameraPosition;

                if (team != TeamType.AutoAssign)
                {
                    _cameraSet = true;
                }
            }        
        }

        private void SetCameraForAutoAssignedTeam()
        {
            if (!_cameraSet)
            {
                if (_localChampQuery.TryGetSingletonEntity<OwnerChampionTag>(out var localChampion))
                {
                    var team = _entityManager.GetComponentData<GameTeam>(localChampion).Value;
                    var cameraPosition = team switch
                    {
                        TeamType.Blue => blueTeamPosition,
                        TeamType.Red => redTeamPosition,
                        _ => spectatorPosition
                    };
                    transform.position = cameraPosition;
                    
                    _cameraSet = true;
                }
            }
        }
        
        private void MoveCamera()
        {
            if (InScreenLeft)
            {
                transform.position += Vector3.left * (camSpeed * Time.deltaTime);
            }

            if (InScreenRight)
            {
                transform.position += Vector3.right * (camSpeed * Time.deltaTime);
            }

            if (InScreenTop)
            {
                transform.position += Vector3.back * (camSpeed * Time.deltaTime);
            }

            if (InScreenBottom)
            {
                transform.position += Vector3.forward * (camSpeed * Time.deltaTime);
            }
            
            if (!cameraBounds.Contains(transform.position))
            {
                transform.position = cameraBounds.ClosestPoint(transform.position);
            }
        }

        private void ZoomCamera()
        {
            if (Mathf.Abs(Input.mouseScrollDelta.y) > float.Epsilon)
            {
                _transposer.m_CameraDistance -= Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
                _transposer.m_CameraDistance =
                    Mathf.Clamp(_transposer.m_CameraDistance, minZoomDistance, maxZoomDistance);
            }
        }
    }
}