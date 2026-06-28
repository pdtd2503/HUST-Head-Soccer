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

   [Header("Map Boundaries")]
private const float MAP_LEFT = -8.2f;   // giới hạn trái map
private const float MAP_RIGHT = 8.4f;   // giới hạn phải map
private const float MAP_BOTTOM = -3.7f; // giới hạn dưới map

private Vector3 GetSafePosition(PlayerController2D playerController, Vector3 targetPosition)
{
    Collider2D col = playerController.GetComponent<Collider2D>();
    float radius = col != null ? col.bounds.extents.x : 0.5f;

    // Clamp vị trí đích trong giới hạn map trước
    targetPosition.x = Mathf.Clamp(targetPosition.x, MAP_LEFT + radius, MAP_RIGHT - radius);
    targetPosition.y = Mathf.Max(targetPosition.y, MAP_BOTTOM + radius);

    // Kiểm tra vị trí đích
    if (IsPositionClear(targetPosition, radius, playerController))
    {
        return targetPosition;
    }

    // Thử các vị trí fallback
    Vector3[] fallbackOffsets = {
        new Vector3(1.5f, 0f, 0f),
        new Vector3(-1.5f, 0f, 0f),
        new Vector3(2f, 0f, 0f),
        new Vector3(-2f, 0f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(1.5f, 1f, 0f),
        new Vector3(-1.5f, 1f, 0f),
    };

        foreach (Vector3 offset in fallbackOffsets)
        {
            Vector3 fallbackPos = targetPosition + offset;

            // Clamp fallback cũng trong giới hạn map
            fallbackPos.x = Mathf.Clamp(fallbackPos.x, MAP_LEFT + radius, MAP_RIGHT - radius);
            fallbackPos.y = Mathf.Max(fallbackPos.y, MAP_BOTTOM + radius);

            if (IsPositionClear(fallbackPos, radius, playerController))
            {
                return fallbackPos;
            }
        }

        // Fallback cuối: spawn ở giữa map
        Debug.LogWarning("SEEE: Không tìm được vị trí an toàn, spawn ở giữa map!");
        return new Vector3(0f, 0f, 0f);
        }

        private bool IsPositionClear(Vector3 position, float radius, PlayerController2D playerController)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius + 0.1f); // thêm 0.1f margin

            foreach (Collider2D hit in hits)
            {
                if (hit.isTrigger) continue;
                if (hit.gameObject == playerController.gameObject) continue;
                if (hit.gameObject.CompareTag("Ball")) continue; // bỏ qua bóng

                return false;
            }

            return true;
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