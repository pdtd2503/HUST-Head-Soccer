using System.Collections;
using UnityEngine;

public class SEEESkill : MonoBehaviour
{
    private const float BALL_OFFSET = 0.75f;
    private GameObject portalPrefab;

    private void Awake()
    {
        portalPrefab = Resources.Load<GameObject>("Portal");

        if (portalPrefab == null)
        {
            Debug.LogError("Không tìm thấy Portal Prefab trong Resources!");
        }
    }

    public void UseSkill(PlayerController2D playerController, Transform ball)
{
    if (playerController == null || ball == null)
    {
        Debug.LogWarning("SEEE: playerController hoặc ball là null!");
        return;
    }

    int attackDirection = playerController.GetAttackDirection();
    Vector3 targetPosition = playerController.transform.position;
    targetPosition.x = ball.position.x - attackDirection * BALL_OFFSET;
    targetPosition.y = ball.position.y;

    // Freeze game
    Time.timeScale = 0f;

    GameObject portalA = Instantiate(portalPrefab, playerController.transform.position, Quaternion.identity);
    PortalController portalAController = portalA.GetComponent<PortalController>();

    // Unfreeze sau khi toàn bộ sequence xong
    portalAController.StartTeleportSequence(playerController, targetPosition, portalPrefab, () =>
    {
        Time.timeScale = 1f;
    });
}
}