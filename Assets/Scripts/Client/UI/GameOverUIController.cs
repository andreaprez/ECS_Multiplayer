using ECS_Multiplayer.Common;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ECS_Multiplayer.Client.UI
{
    public class GameOverUIController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverText;
        [SerializeField] private Button returnToMenuButton;
        [SerializeField] private Button quitButton;

        private EntityQuery _networkConnectionQuery;

        private void OnEnable()
        {
            returnToMenuButton.onClick.AddListener(ReturnToMenu);
            quitButton.onClick.AddListener(QuitGame);

            if (World.DefaultGameObjectInjectionWorld == null)
                return;

            _networkConnectionQuery = World.DefaultGameObjectInjectionWorld.EntityManager
                .CreateEntityQuery(typeof(NetworkStreamConnection));

            var gameOverSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
            gameOverSystem.OnGameOver += ShowGameOverUI;
        }

        private void OnDisable()
        {
            if (World.DefaultGameObjectInjectionWorld == null)
                return;
            
            var gameOverSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
            gameOverSystem.OnGameOver -= ShowGameOverUI;
        }

        private void ReturnToMenu()
        {
            if (_networkConnectionQuery.TryGetSingletonEntity<NetworkStreamConnection>(out var networkConnectionEntity))
            {
                World.DefaultGameObjectInjectionWorld.EntityManager
                    .AddComponent<NetworkStreamRequestDisconnect>(networkConnectionEntity);
            }
            World.DisposeAllWorlds();
            SceneManager.LoadScene(0);
        }
        
        private void QuitGame()
        {
            Application.Quit();
        }
        
        private void ShowGameOverUI(TeamType winningTeam)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = $"{winningTeam.ToString()} Team Wins!";
        }
    }
}