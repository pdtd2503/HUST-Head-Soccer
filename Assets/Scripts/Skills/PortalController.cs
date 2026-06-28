using System.Collections;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    private Animator animator;
    private const float PORTAL_DURATION = 0.6f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.Play("PortalOpen", 0, 0f);
    }

    public void StartTeleportSequence(PlayerController2D playerController, Vector3 targetPosition, GameObject portalPrefab, System.Action onComplete)
    {
        StartCoroutine(TeleportSequenceRoutine(playerController, targetPosition, portalPrefab, onComplete));
    }

   private IEnumerator TeleportSequenceRoutine(PlayerController2D playerController, Vector3 targetPosition, GameObject portalPrefab, System.Action onComplete)
{
    yield return new WaitForSecondsRealtime(PORTAL_DURATION);

    playerController.gameObject.SetActive(false);

    // Kiểm tra vị trí đích có hợp lệ không
    Vector3 safePosition = GetSafePosition(playerController, targetPosition);

    playerController.transform.position = safePosition;

    GameObject portalB = Instantiate(portalPrefab, safePosition, Quaternion.identity);
    PortalController portalBController = portalB.GetComponent<PortalController>();

    portalBController.StartCoroutine(portalBController.FinishTeleport(playerController, onComplete));

    Destroy(gameObject);
}

    private Vector3 GetSafePosition(PlayerController2D playerController, Vector3 targetPosition)
    {
        // Lấy kích thước collider của player
        Collider2D col = playerController.GetComponent<Collider2D>();
        float radius = col != null ? col.bounds.extents.x : 0.5f;

        // Kiểm tra vị trí đích có bị chồng lên vật cản không
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, radius);

        bool isClear = true;
        foreach (Collider2D hit in hits)
        {
            // Bỏ qua chính player và trigger
            if (hit.isTrigger) continue;
            if (hit.gameObject == playerController.gameObject) continue;

            isClear = false;
            break;
        }

        if (isClear) return targetPosition;

        // Nếu vị trí đích bị chặn, thử tìm vị trí an toàn gần đó
        Vector3[] fallbackOffsets = {
            new Vector3(1f, 0f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(1.5f, 0f, 0f),
            new Vector3(-1.5f, 0f, 0f),
        };

        foreach (Vector3 offset in fallbackOffsets)
        {
            Vector3 fallbackPos = targetPosition + offset;
            Collider2D[] fallbackHits = Physics2D.OverlapCircleAll(fallbackPos, radius);

            bool fallbackClear = true;
            foreach (Collider2D hit in fallbackHits)
            {
                if (hit.isTrigger) continue;
                if (hit.gameObject == playerController.gameObject) continue;

                fallbackClear = false;
                break;
            }

            if (fallbackClear) return fallbackPos;
        }

        // Nếu không tìm được vị trí an toàn, giữ nguyên vị trí cũ
        Debug.LogWarning("SEEE: Không tìm được vị trí an toàn, giữ nguyên vị trí cũ!");
        return playerController.transform.position;
    }

    public IEnumerator FinishTeleport(PlayerController2D playerController, System.Action onComplete)
    {
        yield return new WaitForSecondsRealtime(PORTAL_DURATION);

        playerController.gameObject.SetActive(true);

        // Unfreeze game
        onComplete?.Invoke();

        Destroy(gameObject);
    }
}