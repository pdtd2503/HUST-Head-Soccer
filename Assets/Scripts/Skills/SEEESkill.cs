using System.Collections;
using UnityEngine;

public class SEEESkill : MonoBehaviour
{
    [SerializeField] private float ballOffsetX = 0.75f; // khoảng cách ngang, chỉnh trong Inspector
    [SerializeField] private float ballOffsetY = 0f;  // khoảng cách dọc, chỉnh trong Inspector

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

        AudioManager.Instance?.PlaySkillSEEE(1.5f);

        int attackDirection = playerController.GetAttackDirection();
        Vector3 targetPosition = playerController.transform.position;
        targetPosition.x = ball.position.x - attackDirection * ballOffsetX;
        targetPosition.y = ball.position.y - ballOffsetY; // thêm offset Y

        Time.timeScale = 0f;

        GameObject portalA = Instantiate(portalPrefab, playerController.transform.position, Quaternion.identity);
        PortalController portalAController = portalA.GetComponent<PortalController>();

        portalAController.StartTeleportSequence(playerController, targetPosition, portalPrefab, () =>
        {
            Time.timeScale = 1f;
        });
    }
}