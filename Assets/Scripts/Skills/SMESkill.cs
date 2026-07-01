using System.Collections;
using UnityEngine;

public class SMESkill : MonoBehaviour
{
    private const float WALL_DURATION = 2f;
    private const float WALL_GOAL_OFFSET_P1 = 1.1f;  // offset cho Player 1
    private const float WALL_GOAL_OFFSET_P2 = 1.5f;  // offset riêng cho Player 2, chỉnh số này
    private const float APPEAR_DURATION = 0.5f;
    private const float GROUND_Y = -0.2f;

    private GameObject rockWallPrefab;

    private void Awake()
    {
        rockWallPrefab = Resources.Load<GameObject>("RockWall");

        if (rockWallPrefab == null)
        {
            Debug.LogError("Không tìm thấy RockWall Prefab trong Resources!");
        }
    }

    public void UseSkill(PlayerController2D playerController, MatchManager matchManager)
    {
        if (playerController == null || matchManager == null) return;

        AudioManager.Instance?.PlaySkillSME();

        Transform ownGoal = GetOwnGoal(playerController, matchManager);

        if (ownGoal == null)
        {
            Debug.LogWarning("SME skill could not find own goal.");
            return;
        }

        Vector3 wallPosition = ownGoal.position;
        int attackDirection = playerController.GetAttackDirection();

        if (attackDirection > 0)
        {
            // Player 1
            wallPosition.x += WALL_GOAL_OFFSET_P1;
        }
        else
        {
            // Player 2
            wallPosition.x -= WALL_GOAL_OFFSET_P2;
        }

        wallPosition.y = GROUND_Y;

        GameObject wall = Instantiate(rockWallPrefab, wallPosition, Quaternion.identity);
        Vector3 originalScale = wall.transform.localScale;
        wall.transform.localScale = new Vector3(originalScale.x, 0f, originalScale.z);

        SpriteRenderer sr = wall.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = attackDirection < 0;
        }

        StartCoroutine(RockWallRoutine(wall, originalScale, wallPosition));
    }

    private IEnumerator RockWallRoutine(GameObject wall, Vector3 originalScale, Vector3 spawnPosition)
    {
        float[] steps = { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };
        float stepDelay = APPEAR_DURATION / steps.Length;

        foreach (float step in steps)
        {
            if (wall == null) yield break;

            wall.transform.localScale = new Vector3(
                originalScale.x,
                originalScale.y * step,
                originalScale.z
            );
            wall.transform.position = new Vector3(
                spawnPosition.x,
                spawnPosition.y + (originalScale.y * step) / 2f,
                spawnPosition.z
            );

            yield return new WaitForSecondsRealtime(stepDelay);
        }

        yield return new WaitForSecondsRealtime(WALL_DURATION);

        float[] stepsDown = { 0.8f, 0.6f, 0.4f, 0.2f, 0f };
        foreach (float step in stepsDown)
        {
            if (wall == null) yield break;

            wall.transform.localScale = new Vector3(
                originalScale.x,
                originalScale.y * step,
                originalScale.z
            );
            wall.transform.position = new Vector3(
                spawnPosition.x,
                spawnPosition.y + (originalScale.y * step) / 2f,
                spawnPosition.z
            );

            yield return new WaitForSecondsRealtime(stepDelay);
        }

        Destroy(wall);
    }

    private Transform GetOwnGoal(PlayerController2D playerController, MatchManager matchManager)
    {
        int attackDirection = playerController.GetAttackDirection();

        if (attackDirection > 0)
        {
            if (matchManager.leftGoalTrigger == null) return null;
            return matchManager.leftGoalTrigger.transform;
        }

        if (matchManager.rightGoalTrigger == null) return null;
        return matchManager.rightGoalTrigger.transform;
    }
}