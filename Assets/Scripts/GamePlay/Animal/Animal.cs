using System;
using System.Collections;
using PickAndMatch.Gameplay.Entities;
using UnityEngine;

namespace PickAndMatch.Gameplay.Animal
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Animal : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer iconRenderer;

        [Header("Animation")]
        [SerializeField] private float flipDuration = 0.2f;
        [SerializeField] private float disappearDuration = 0.2f;

        private AnimalData data;
        private SpriteRenderer frameRenderer;
        private Sprite backFrameSprite;
        private Sprite frontFrameSprite;

        private bool isRevealed;
        private bool isAnimating;
        private Coroutine animationRoutine;

        private Vector3 baseScale = Vector3.one;

        public event Action<Animal> OnClicked;

        public AnimalType Type
        {
            get
            {
                if (data == null)
                    return AnimalType.Animal01;

                return data.Type;
            }
        }

        public AnimalData Data => data;

        public bool IsRevealed => isRevealed;

        public void Initialize(
            AnimalData animalData,
            Sprite backFrame,
            Sprite frontFrame)
        {
            if (animalData == null)
            {
                Debug.LogError("AnimalData is null.");
                return;
            }

            data = animalData;
            backFrameSprite = backFrame;
            frontFrameSprite = frontFrame;

            frameRenderer = GetComponent<SpriteRenderer>();

            isRevealed = false;
            isAnimating = false;
            animationRoutine = null;

            baseScale = Vector3.one;
            transform.localScale = baseScale;

            if (backFrameSprite != null)
            {
                frameRenderer.sprite = backFrameSprite;
            }

            if (iconRenderer != null)
            {
                iconRenderer.sprite = data.Sprite;
                iconRenderer.enabled = false;
            }
        }

        public void SetCardScale(float scale)
        {
            scale = Mathf.Max(0.01f, scale);

            baseScale = Vector3.one * scale;

            if (!isAnimating)
            {
                transform.localScale = baseScale;
            }
        }

        public void SetCardScale(Vector3 scale)
        {
            baseScale = scale;

            if (!isAnimating)
            {
                transform.localScale = baseScale;
            }
        }

        public Vector3 GetCardScale()
        {
            return baseScale;
        }

        public void Reveal()
        {
            if (isRevealed || isAnimating)
                return;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(
                    AudioManager.Instance.Swap
                );
            }

            StopCurrentAnimation();

            animationRoutine =
                StartCoroutine(
                    FlipRoutine(true)
                );
        }

        public void Hide()
        {
            if (isAnimating)
                return;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(
                    AudioManager.Instance.Swap
                );
            }

            StopCurrentAnimation();

            animationRoutine =
                StartCoroutine(
                    FlipRoutine(false)
                );
        }

        public void Disappear(Action onComplete = null)
        {
            StopCurrentAnimation();

            animationRoutine =
                StartCoroutine(
                    DisappearRoutine(onComplete)
                );
        }

        private void StopCurrentAnimation()
        {
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
                animationRoutine = null;
            }

            isAnimating = false;
        }

        private IEnumerator FlipRoutine(bool toFront)
        {
            isAnimating = true;
            isRevealed = toFront;

            float half = flipDuration * 0.5f;

            yield return ScaleX(1f, 0f, half);

            if (toFront)
            {
                if (frontFrameSprite != null)
                {
                    frameRenderer.sprite = frontFrameSprite;
                }

                if (iconRenderer != null)
                {
                    iconRenderer.enabled = true;
                }
            }
            else
            {
                if (backFrameSprite != null)
                {
                    frameRenderer.sprite = backFrameSprite;
                }

                if (iconRenderer != null)
                {
                    iconRenderer.enabled = false;
                }
            }

            yield return ScaleX(0f, 1f, half);

            transform.localScale = baseScale;

            isAnimating = false;
            animationRoutine = null;
        }

        private IEnumerator DisappearRoutine(
            Action onComplete)
        {
            isAnimating = true;

            Vector3 startScale =
                transform.localScale;

            float elapsed = 0f;

            while (elapsed < disappearDuration)
            {
                elapsed += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        elapsed /
                        disappearDuration
                    );

                transform.localScale =
                    Vector3.Lerp(
                        startScale,
                        Vector3.zero,
                        t
                    );

                yield return null;
            }

            transform.localScale =
                Vector3.zero;

            onComplete?.Invoke();

            Destroy(gameObject);
        }

        private IEnumerator ScaleX(
            float from,
            float to,
            float duration)
        {
            if (duration <= 0f)
            {
                Vector3 finalScale =
                    baseScale;

                finalScale.x *= to;

                transform.localScale =
                    finalScale;

                yield break;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        elapsed /
                        duration
                    );

                float currentX =
                    Mathf.Lerp(
                        from,
                        to,
                        t
                    );

                Vector3 scale =
                    baseScale;

                scale.x *= currentX;

                transform.localScale =
                    scale;

                yield return null;
            }

            Vector3 endScale =
                baseScale;

            endScale.x *= to;

            transform.localScale =
                endScale;
        }

        private void OnMouseDown()
        {
            if (isRevealed || isAnimating)
                return;

            OnClicked?.Invoke(this);
        }
    }
}