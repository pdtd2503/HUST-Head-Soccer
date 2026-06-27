using System.Collections;
using UnityEngine;

public class SCLSSkill : MonoBehaviour
{
    private const float SHRINK_SCALE_MULTIPLIER = 0.5f;
    private const float SHRINK_DURATION = 4f;
    private const float FLASH_DURATION = 0.05f;
    private const int FLASH_COUNT = 4;

    private const float THROW_SPEED = 12f;
    private const float THROW_SIZE = 0.3f;
    private const float ARC_HEIGHT = 1.5f;
    private const float SPIN_SPEED = 540f;

    private GameObject vialPrefab; // đổi từ Sprite sang GameObject

    private void Awake()
    {
        vialPrefab = Resources.Load<GameObject>("ChemicalVial"); // đổi <Sprite> thành <GameObject>

        if (vialPrefab == null)
        {
            Debug.LogError("Không tìm thấy ChemicalVial Prefab trong Resources!");
        }
    }
    public void UseSkill(PlayerController2D playerController, PlayerController2D opponentController)
    {
        if (opponentController == null || playerController == null)
        {
            Debug.LogWarning("SCLS skill could not find opponent or player.");
            return;
        }
        AudioManager.Instance?.PlaySkillSCLS();
        Time.timeScale = 0f;

        StartCoroutine(ThrowVialRoutine(playerController, opponentController));
    }

    private IEnumerator ThrowVialRoutine(PlayerController2D playerController, PlayerController2D opponentController)
    {
        AudioManager.Instance?.PlaySkillSCLSThrow();
        GameObject vial = Instantiate(vialPrefab, playerController.transform.position, Quaternion.identity);

        SpriteRenderer sr = vial.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 20;
        }

        Vector3 targetPosition = opponentController.transform.position;
        Vector3 startPosition = vial.transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float travelTime = distance / THROW_SPEED;

        float elapsed = 0f;

        while (elapsed < travelTime)
        {
            if (vial == null) yield break;

            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / travelTime);

            Vector3 linearPos = Vector3.Lerp(startPosition, targetPosition, t);
            float heightOffset = ARC_HEIGHT * 4f * t * (1f - t);

            vial.transform.position = new Vector3(
                linearPos.x,
                linearPos.y + heightOffset,
                linearPos.z
            );

            vial.transform.Rotate(0f, 0f, SPIN_SPEED * Time.unscaledDeltaTime);

            yield return null;
        }
        AudioManager.Instance?.PlaySkillSCLSBreak();
        Destroy(vial);

        yield return StartCoroutine(ApplyShrinkEffect(opponentController));

        Time.timeScale = 1f;
    }

    private IEnumerator ApplyShrinkEffect(PlayerController2D opponentController)
    {
        SclsShrinkRuntime shrinkRuntime = opponentController.GetComponent<SclsShrinkRuntime>();

        if (shrinkRuntime == null)
        {
            shrinkRuntime = opponentController.gameObject.AddComponent<SclsShrinkRuntime>();
        }

        yield return shrinkRuntime.ApplyShrinkAndWaitAppear(
            SHRINK_SCALE_MULTIPLIER, SHRINK_DURATION, FLASH_DURATION, FLASH_COUNT
        );
    }
}

class SclsShrinkRuntime : MonoBehaviour
{
    private Coroutine shrinkCoroutine;
    private Vector3 originalScale;
    private bool hasOriginalScale;

    public IEnumerator ApplyShrinkAndWaitAppear(float scaleMultiplier, float duration, float flashDuration, int flashCount)
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
        AudioManager.Instance?.PlaySkillSCLSPoison();
        SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (allRenderers.Length > 0)
        {
            Color[] originalColors = new Color[allRenderers.Length];
            for (int i = 0; i < allRenderers.Length; i++)
            {
                originalColors[i] = allRenderers[i].color;
            }

            Color flashColor = new Color(0.8f, 0.2f, 1f);

            for (int i = 0; i < flashCount; i++)
            {
                foreach (SpriteRenderer renderer in allRenderers)
                {
                    if (renderer != null) renderer.color = flashColor;
                }
                yield return new WaitForSecondsRealtime(flashDuration);

                for (int j = 0; j < allRenderers.Length; j++)
                {
                    if (allRenderers[j] != null) allRenderers[j].color = originalColors[j];
                }
                yield return new WaitForSecondsRealtime(flashDuration);
            }
        }

        float[] steps = { 0.85f, 0.65f, 0.75f, 0.55f, scaleMultiplier };

        foreach (float step in steps)
        {
            transform.localScale = new Vector3(
                originalScale.x * step,
                originalScale.y * step,
                originalScale.z
            );
            yield return new WaitForSecondsRealtime(0.05f);
        }

        PlayerController2D playerController = GetComponent<PlayerController2D>();
        shrinkCoroutine = StartCoroutine(ShrinkDurationAndRestoreRoutine(duration, scaleMultiplier, playerController));
    }

    private IEnumerator ShrinkDurationAndRestoreRoutine(float duration, float scaleMultiplier, PlayerController2D playerController)
    {
        if (playerController != null)
        {
            playerController.SetTemporarySpeedStars(1, duration);
        }

        yield return new WaitForSeconds(duration);

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
        if (hasOriginalScale) transform.localScale = originalScale;
    }

    private void OnDestroy()
    {
        if (hasOriginalScale) transform.localScale = originalScale;
    }
}