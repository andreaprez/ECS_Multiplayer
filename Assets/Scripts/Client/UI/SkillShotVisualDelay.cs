using System.Collections;
using UnityEngine;

namespace ECS_Multiplayer.Client.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SkillShotVisualDelay : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private int delayFrameCount = 1;

        private void Awake()
        {
            spriteRenderer.enabled = false;
        }

        private IEnumerator Start()
        {
            for (var i = 0; i < delayFrameCount; i++)
            {
                yield return null;
            }

            spriteRenderer.enabled = true;
        }
    }
}