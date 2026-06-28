using System.Collections;
using UnityEngine;

public class GoalEffectManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform goalImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Scale Bounce")]
    [SerializeField] private float startScale = 0.2f;
    [SerializeField] private float overshootScale = 1.2f;
    [SerializeField] private float settleScale = 1.0f;

    [SerializeField] private float popUpDuration = 0.12f;
    [SerializeField] private float overshootBackDuration = 0.08f;
    [SerializeField] private float settleDuration = 0.08f;

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeX = 16f;
    [SerializeField] private float shakeY = 8f;

    [Header("Timing")]
    [SerializeField] private float holdDuration = 0.8f;

    private Vector2 originalAnchoredPosition;
    private Coroutine effectCoroutine;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (goalImage != null)
        {
            originalAnchoredPosition = goalImage.anchoredPosition;
            goalImage.localScale = Vector3.zero;
        }

        HideInstant();
    }

    public void PlayGoalEffect()
    {
        if (goalImage == null || canvasGroup == null)
        {
            Debug.LogWarning("GoalEffectManager is missing references.");
            return;
        }

        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        effectCoroutine = StartCoroutine(GoalEffectRoutine());
    }

    private IEnumerator GoalEffectRoutine()
    {
        canvasGroup.alpha = 1f;
        goalImage.anchoredPosition = originalAnchoredPosition;
        goalImage.localScale = Vector3.one * startScale;

        yield return ScaleTo(overshootScale, popUpDuration);
        yield return ScaleTo(0.95f, overshootBackDuration);
        yield return ScaleTo(settleScale, settleDuration);

        yield return ShakeImage();

        yield return new WaitForSeconds(holdDuration);

        HideInstant();
        effectCoroutine = null;
    }

    private IEnumerator ScaleTo(float targetScale, float duration)
    {
        Vector3 start = goalImage.localScale;
        Vector3 target = Vector3.one * targetScale;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth hơn Linear một chút
            t = 1f - Mathf.Pow(1f - t, 3f);

            goalImage.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        goalImage.localScale = target;
    }

    private IEnumerator ShakeImage()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float offsetX = Random.Range(-shakeX, shakeX);
            float offsetY = Random.Range(-shakeY, shakeY);

            goalImage.anchoredPosition = originalAnchoredPosition + new Vector2(offsetX, offsetY);

            yield return null;
        }

        goalImage.anchoredPosition = originalAnchoredPosition;
    }

    private void HideInstant()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (goalImage != null)
        {
            goalImage.anchoredPosition = originalAnchoredPosition;
            goalImage.localScale = Vector3.zero;
        }
    }
}