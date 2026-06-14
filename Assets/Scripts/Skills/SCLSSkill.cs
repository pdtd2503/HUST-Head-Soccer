using System.Collections;
using UnityEngine;

public class SCLSSkill : MonoBehaviour
{
    private const float SHRINK_SCALE_MULTIPLIER = 0.5f;
    private const float SHRINK_DURATION = 4f;
    private const float FLASH_DURATION = 0.05f;
    private const int FLASH_COUNT = 4;

    public void UseSkill(PlayerController2D opponentController)
    {
        if (opponentController == null)
        {
            Debug.LogWarning("SCLS skill could not find opponent.");
            return;
        }

        SclsShrinkRuntime shrinkRuntime = opponentController.GetComponent<SclsShrinkRuntime>();

        if (shrinkRuntime == null)
        {
            shrinkRuntime = opponentController.gameObject.AddComponent<SclsShrinkRuntime>();
        }

        shrinkRuntime.ApplyShrink(SHRINK_SCALE_MULTIPLIER, SHRINK_DURATION, FLASH_DURATION, FLASH_COUNT);
    }
}

class SclsShrinkRuntime : MonoBehaviour
{
    private Coroutine shrinkCoroutine;
    private Vector3 originalScale;
    private bool hasOriginalScale;

    public void ApplyShrink(float scaleMultiplier, float duration, float flashDuration, int flashCount)
    {
        if (!hasOriginalScale)
        {
            originalScale = transform.localScale;
            hasOriginalScale = true;
        }

        if (shrinkCoroutine != null)
        {
            StopCoroutine(shrinkCoroutine);
        }

        shrinkCoroutine = StartCoroutine(ShrinkRoutine(scaleMultiplier, duration, flashDuration, flashCount));
    }

    private IEnumerator ShrinkRoutine(float scaleMultiplier, float duration, float flashDuration, int flashCount)
    {
        PlayerController2D playerController = GetComponent<PlayerController2D>();
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        // Bước 1: Flash màu tím
        if (sr != null)
        {
            Color originalColor = sr.color;
            Color flashColor = new Color(0.8f, 0.2f, 1f);

            for (int i = 0; i < flashCount; i++)
            {
                sr.color = flashColor;
                yield return new WaitForSeconds(flashDuration);
                sr.color = originalColor;
                yield return new WaitForSeconds(flashDuration);
            }
        }

        // Bước 2: Scale giật cục thu nhỏ
        float[] steps = { 0.85f, 0.65f, 0.75f, 0.55f, scaleMultiplier };

        foreach (float step in steps)
        {
            transform.localScale = new Vector3(
                originalScale.x * step,
                originalScale.y * step,
                originalScale.z
            );
            yield return new WaitForSeconds(0.05f);
        }

        // Giảm tốc độ và jump khi bị shrink
        if (playerController != null)
        {
            playerController.SetTemporarySpeedStars(1, duration); // giảm speed xuống 1 sao
        }

        // Giữ nguyên shrink trong duration giây
        yield return new WaitForSeconds(duration);

        // Bước 3: Phục hồi kích thước giật cục
        float[] stepsBack = { 0.55f, 0.75f, 0.65f, 0.85f, 1.0f };

        foreach (float step in stepsBack)
        {
            transform.localScale = new Vector3(
                originalScale.x * step,
                originalScale.y * step,
                originalScale.z
            );
            yield return new WaitForSeconds(0.05f);
        }

        transform.localScale = originalScale;
        shrinkCoroutine = null;
    }

    private void OnDisable()
    {
        if (hasOriginalScale)
            transform.localScale = originalScale;
    }

    private void OnDestroy()
    {
        if (hasOriginalScale)
            transform.localScale = originalScale;
    }
}