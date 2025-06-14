﻿using UnityEngine;
using UnityEngine.UI;

namespace ECS_Multiplayer.Client.UI
{
    public class AbilityCooldownUIController : MonoBehaviour
    {
        public static AbilityCooldownUIController Instance;

        [SerializeField] private Image _aoeAbilityMask;
        [SerializeField] private Image _skillShotAbilityMask;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            _aoeAbilityMask.fillAmount = 0f;
            _skillShotAbilityMask.fillAmount = 0f;
        }
        
        public void UpdateAoeCooldownMask(float fillAmount)
        {
            _aoeAbilityMask.fillAmount = fillAmount;
        }
        
        public void UpdateSkillShotCooldownMask(float fillAmount)
        {
            _skillShotAbilityMask.fillAmount = fillAmount;
        }
    }
}