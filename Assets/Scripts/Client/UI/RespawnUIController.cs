using ECS_Multiplayer.Common.Respawn;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace ECS_Multiplayer.Client.UI
{
    public class RespawnUIController : MonoBehaviour
    {
        [SerializeField] private GameObject respawnPanel;
        [SerializeField] private TextMeshProUGUI respawnCountdownText;

        private void OnEnable()
        {
            respawnPanel.SetActive(false);
            
            if (World.DefaultGameObjectInjectionWorld == null)
                return;

            var respawnSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RespawnChampionSystem>();
            if (respawnSystem != null)
            {
                respawnSystem.OnUpdateRespawnCountdown += UpdateRespawnCountdownText;
                respawnSystem.OnRespawn += CloseRespawnPanel;
            }
        }

        private void OnDisable()
        {
            if (World.DefaultGameObjectInjectionWorld == null)
                return;

            var respawnSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RespawnChampionSystem>();
            if (respawnSystem != null)
            {
                respawnSystem.OnUpdateRespawnCountdown -= UpdateRespawnCountdownText;
                respawnSystem.OnRespawn -= CloseRespawnPanel;
            }
        }

        private void UpdateRespawnCountdownText(int secondsToRespawn)
        {
            if (!respawnPanel.activeSelf)
                respawnPanel.SetActive(true);

            respawnCountdownText.text = secondsToRespawn.ToString();
        }

        private void CloseRespawnPanel()
        {
            respawnPanel.SetActive(false);
        }
    }
}