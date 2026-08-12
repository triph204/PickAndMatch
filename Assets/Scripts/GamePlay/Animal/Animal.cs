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
        [Tooltip("SpriteRenderer con vật, đặt trên Child GameObject riêng, Sorting Order/Layer PHẢI giống root và cao hơn.")]
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

            transform.localScale = Vector3.one;

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

        // Lật thẻ lên (animation), hiện con vật.
        public void Reveal()
        {
            if (isRevealed || isAnimating)
                return;

            StopCurrentAnimation();
            animationRoutine =
                StartCoroutine(FlipRoutine(toFront: true));
        }

        // Úp thẻ lại (animation), dùng khi chọn sai.
        public void Hide()
        {
            if (isAnimating)
                return;

            StopCurrentAnimation();
            animationRoutine =
                StartCoroutine(FlipRoutine(toFront: false));
        }

        // Đúng cặp -> chạy animation biến mất rồi tự Destroy.
        public void Disappear(Action onComplete = null)
        {
            StopCurrentAnimation();
            animationRoutine =
                StartCoroutine(DisappearRoutine(onComplete));
        }

        private void StopCurrentAnimation()
        {
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
                animationRoutine = null;
            }
        }

        private IEnumerator FlipRoutine(bool toFront)
        {
            isAnimating = true;
            isRevealed = toFront;

            float half = flipDuration * 0.5f;

            // Nửa đầu: co bề ngang về 0 (giống thẻ đang nghiêng dần).
            yield return ScaleX(1f, 0f, half);

            // Tại điểm giữa (thẻ "mỏng dính") thì đổi mặt + đổi icon.
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

            // Nửa sau: giãn bề ngang trở lại 1 (thẻ mở ra hết cỡ).
            yield return ScaleX(0f, 1f, half);

            isAnimating = false;
            animationRoutine = null;
        }

        private IEnumerator DisappearRoutine(Action onComplete)
        {
            isAnimating = true;

            Vector3 startScale = transform.localScale;
            float elapsed = 0f;

            while (elapsed < disappearDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / disappearDuration);

                transform.localScale =
                    Vector3.Lerp(startScale, Vector3.zero, t);

                yield return null;
            }

            transform.localScale = Vector3.zero;

            onComplete?.Invoke();

            Destroy(gameObject);
        }

        private IEnumerator ScaleX(float from, float to, float duration)
        {
            Vector3 scale = transform.localScale;

            if (duration <= 0f)
            {
                scale.x = to;
                transform.localScale = scale;
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                scale.x = Mathf.Lerp(from, to, t);
                transform.localScale = scale;

                yield return null;
            }

            scale.x = to;
            transform.localScale = scale;
        }

        private void OnMouseDown()
        {
            if (isRevealed || isAnimating)
                return;

            OnClicked?.Invoke(this);
        }
    }
}