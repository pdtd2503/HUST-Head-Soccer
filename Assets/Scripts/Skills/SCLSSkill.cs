using System.Collections;
using UnityEngine;

public static class SCLSSkill
{
    private const float SHRINK_SCALE_MULTIPLIER = 0.5f;
    private const float SHRINK_DURATION = 4f;

    public static void UseSkill(PlayerController2D opponentController)
    {
        if (opponentController == null)
        {
            Debug.LogWarning("SCLS skill could not find opponent.");
            return;
        }

        SclsShrinkRuntime runtime =
            opponentController.GetComponent<SclsShrinkRuntime>();

        if (runtime == null)
        {
            runtime =
                opponentController.gameObject.AddComponent<SclsShrinkRuntime>();
        }

        runtime.ApplyShrink(
            SHRINK_SCALE_MULTIPLIER,
            SHRINK_DURATION
        );
    }
}

public class SclsShrinkRuntime : MonoBehaviour
{
    private Coroutine shrinkCoroutine;

    private Vector3 originalScale;
    private bool hasOriginalScale;

    public void ApplyShrink(float scaleMultiplier, float duration)
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

        shrinkCoroutine =
            StartCoroutine(ShrinkRoutine(scaleMultiplier, duration));
    }

    private IEnumerator ShrinkRoutine(float scaleMultiplier, float duration)
    {
        transform.localScale = new Vector3(
            originalScale.x * scaleMultiplier,
            originalScale.y * scaleMultiplier,
            originalScale.z
        );

        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
        shrinkCoroutine = null;
    }

    private void OnDisable()
    {
        RestoreOriginalScale();
    }

    private void OnDestroy()
    {
        RestoreOriginalScale();
    }

    private void RestoreOriginalScale()
    {
        if (hasOriginalScale)
        {
            transform.localScale = originalScale;
        }
    }
}