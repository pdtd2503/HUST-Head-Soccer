using System.Collections;
using UnityEngine;

public class SMESkill : MonoBehaviour
{
    private const float WALL_DURATION = 2f;
    private const float WALL_GOAL_OFFSET = 1f;
    private const float APPEAR_DURATION = 0.6f; // thời gian mọc lên

    private GameObject rockWallPrefab;

    private void Awake()
    {
        rockWallPrefab = Resources.Load<GameObject>("RockWall");

        if (rockWallPrefab == null)
        {
            Debug.LogError("Không tìm thấy RockWall Prefab trong Resources!");
        }
    }
    private const float GROUND_Y = 0f;
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
            wallPosition.x += WALL_GOAL_OFFSET;
        else
            wallPosition.x -= WALL_GOAL_OFFSET;
        wallPosition.y = GROUND_Y;
        GameObject wall = Instantiate(rockWallPrefab, wallPosition, Quaternion.identity);
        Vector3 originalScale = wall.transform.localScale;
        wall.transform.localScale = new Vector3(originalScale.x, 0f, originalScale.z);

        StartCoroutine(RockWallRoutine(wall, originalScale, wallPosition));
    }

    private IEnumerator RockWallRoutine(GameObject wall, Vector3 originalScale, Vector3 spawnPosition)
    {
        // Mọc lên theo từng bước (pixel style, không lerp mượt)
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

        // Giữ nguyên trong WALL_DURATION giây
        yield return new WaitForSecondsRealtime(WALL_DURATION);

        // Chìm xuống
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