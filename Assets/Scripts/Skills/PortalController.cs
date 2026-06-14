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
        playerController.transform.position = targetPosition;

        GameObject portalB = Instantiate(portalPrefab, targetPosition, Quaternion.identity);
        PortalController portalBController = portalB.GetComponent<PortalController>();

        portalBController.StartCoroutine(portalBController.FinishTeleport(playerController, onComplete));

        Destroy(gameObject);
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