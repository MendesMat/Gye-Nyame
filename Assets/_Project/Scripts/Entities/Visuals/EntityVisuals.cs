using UnityEngine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Entities.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EntityVisuals : MonoBehaviour, IEntityVisuals
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Fading Settings")]
        [SerializeField] private float fadeDuration = 1.5f;

        public float FadeDuration => fadeDuration;

        private void Awake()
        {
            if (spriteRenderer != null) return;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetAlpha(float alpha)
        {
            if (spriteRenderer == null) return;
            
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        public void ResetVisuals() => SetAlpha(1f);
    }
}
