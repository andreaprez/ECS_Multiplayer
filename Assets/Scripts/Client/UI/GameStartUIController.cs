using System.Collections;
using ECS_Multiplayer.Common;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ECS_Multiplayer.Client.UI
{
    public class GameStartUIController : MonoBehaviour
    {
        [SerializeField] private GameObject beginGamePanel;
        [SerializeField] private GameObject confirmQuitPanel;
        [SerializeField] private GameObject countdownPanel;
        
        [SerializeField] private Button quitWaitingButton;
        [SerializeField] private Button confirmQuitButton;
        [SerializeField] private Button cancelQuitButton;
        
        [SerializeField] private TextMeshProUGUI playersRemainingText;
        [SerializeField] private TextMeshProUGUI countdownText;

        private EntityQuery _networkConnectionQuery;
        private EntityManager _entityManager;

        private void OnEnable()
        {
            beginGamePanel.SetActive(true);
            
            quitWaitingButton.onClick.AddListener(AttemptQuit);
            confirmQuitButton.onClick.AddListener(ConfirmQuit);
            cancelQuitButton.onClick.AddListener(CancelQuit);
            
            if (World.DefaultGameObjectInjectionWorld == null)
                return;

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _networkConnectionQuery = _entityManager.CreateEntityQuery(typeof(NetworkStreamConnection));

            var startGameSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ClientStartGameSystem>();
            if (startGameSystem != null)
            {
                startGameSystem.OnUpdatePlayersRemainingToStart += UpdatePlayersRemainingText;
                startGameSystem.OnStartGameCountdown += BeginCountdown;
            }

            var countdownSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CountdownToGameStartSystem>();
            if (countdownSystem != null)
            {
                countdownSystem.OnUpdateCountdownText += UpdateCountdownText;
                countdownSystem.OnCountdownEnd += EndCountdown;
            }
        }

        private void OnDisable()
        {
            quitWaitingButton.onClick.RemoveAllListeners();
            confirmQuitButton.onClick.RemoveAllListeners();
            cancelQuitButton.onClick.RemoveAllListeners();

            if (World.DefaultGameObjectInjectionWorld == null)
                return;
            
            var startGameSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ClientStartGameSystem>();
            if (startGameSystem == null)
                return;
            
            startGameSystem.OnUpdatePlayersRemainingToStart -= UpdatePlayersRemainingText;
            startGameSystem.OnStartGameCountdown -= BeginCountdown;
        }

        private void UpdatePlayersRemainingText(int playersRemaining)
        {
            var playersText = playersRemaining == 1 ? "player" : "players";
            playersRemainingText.text = $"Waiting for {playersRemaining.ToString()} more {playersText} to join...";
        }

        private void UpdateCountdownText(int countdownTime)
        {
            countdownText.text = countdownTime.ToString();
        }
        
        private void BeginCountdown()
        {
            beginGamePanel.SetActive(false);
            confirmQuitPanel.SetActive(false);
            countdownPanel.SetActive(true);
        }

        private void EndCountdown()
        {
            countdownPanel.SetActive(false);
        }

        private void AttemptQuit()
        {
            beginGamePanel.SetActive(false);
            confirmQuitPanel.SetActive(true);
        }
        
        private void ConfirmQuit()
        {
            StartCoroutine(DisconnectDelayed());
        }
        
        private void CancelQuit()
        {
            beginGamePanel.SetActive(true);
            confirmQuitPanel.SetActive(false);
        }

        private IEnumerator DisconnectDelayed()
        {
            yield return new WaitForSeconds(1f);

            if (_networkConnectionQuery.TryGetSingletonEntity<NetworkStreamConnection>(out var networkConnectionEntity))
            {
                World.DefaultGameObjectInjectionWorld.EntityManager
                    .AddComponent<NetworkStreamRequestDisconnect>(networkConnectionEntity);
            }
            World.DisposeAllWorlds();
            SceneManager.LoadScene(0);
        }
    }
}