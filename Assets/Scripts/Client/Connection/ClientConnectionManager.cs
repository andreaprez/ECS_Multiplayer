using TMPro;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ECS_Multiplayer.Client.Connection
{
    public class ClientConnectionManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField addressField;
        [SerializeField] private TMP_InputField portField;
        [SerializeField] private TMP_Dropdown connectionModeDropdown;
        [SerializeField] private TMP_Dropdown teamDropdown;
        [SerializeField] private Button connectButton;
        [SerializeField] private TextMeshProUGUI connectButtonText;

        private ushort Port => ushort.Parse(portField.text);
        private string Address => addressField.text;

        private void OnEnable()
        {
            connectionModeDropdown.onValueChanged.AddListener(OnConnectionModeChanged);
            connectButton.onClick.AddListener(OnConnectButtonPressed);
            OnConnectionModeChanged(connectionModeDropdown.value);
        }

        private void OnDisable()
        {
            connectionModeDropdown.onValueChanged.RemoveAllListeners();
            connectButton.onClick.RemoveAllListeners();
        }

        private void OnConnectionModeChanged(int connectionMode)
        {
            string buttonText;
            connectButton.enabled = true;

            switch (connectionMode)
            {
                case 0:
                    buttonText = "Start Host";
                    break;
                case 1:
                    buttonText = "Start Server";
                    break;
                case 2:
                    buttonText = "Start Client";
                    break;
                default:
                    buttonText = "<ERROR>";
                    connectButton.enabled = false;
                    break;
            }
            
            connectButtonText.SetText(buttonText);
        }

        private void OnConnectButtonPressed()
        {
            DestroyLocalSimulationWorld();
            SceneManager.LoadScene(1);
            
            switch (connectionModeDropdown.value)
            {
                case 0:
                    StartServer();
                    StartClient();
                    break;
                case 1:
                    StartServer();
                    break;
                case 2:
                    StartClient();
                    break;
                default:
                    Debug.LogError("Error: Unknown connection mode", gameObject);
                    break;
            }
        }

        private void DestroyLocalSimulationWorld()
        {
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }
        }

        private void StartServer()
        {
            var serverWorld = ClientServerBootstrap.CreateServerWorld("Server World");
            var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);
            {
                using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery((ComponentType.ReadWrite<NetworkStreamDriver>()));
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
            }
        }

        private void StartClient()
        {
            var clientWorld = ClientServerBootstrap.CreateClientWorld("Client World");
            var connectionEndpoint = NetworkEndpoint.Parse(Address, Port);
            {
                using var networkDriverQuery = clientWorld.EntityManager.CreateEntityQuery((ComponentType.ReadWrite<NetworkStreamDriver>()));
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);
            }
            World.DefaultGameObjectInjectionWorld = clientWorld;
        }
    }
}