using UnityEngine;

public class SMESkill : MonoBehaviour
{
    private const float WALL_DURATION = 2f;
    private const float WALL_WIDTH = 0.35f;
    private const float WALL_HEIGHT = 3.5f;
    private const float WALL_GOAL_OFFSET = 0.55f;

    private static Sprite whiteSquareSprite;

    public void UseSkill(PlayerController2D playerController, MatchManager matchManager)
    {
        if (playerController == null || matchManager == null)
        {
            return;
        }

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
            wallPosition.x += WALL_GOAL_OFFSET;
        }
        else
        {
            wallPosition.x -= WALL_GOAL_OFFSET;
        }

        GameObject wall = new GameObject("SME_GoalWall");
        wall.transform.position = wallPosition;
        wall.transform.localScale = new Vector3(WALL_WIDTH, WALL_HEIGHT, 1f);

        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetWhiteSquareSprite();
        spriteRenderer.color = new Color(0.55f, 0.55f, 0.55f, 0.85f);
        spriteRenderer.sortingOrder = 20;

        BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
        wallCollider.isTrigger = false;

        Rigidbody2D wallRb = wall.AddComponent<Rigidbody2D>();
        wallRb.bodyType = RigidbodyType2D.Static;

        Destroy(wall, WALL_DURATION);
    }

    private Transform GetOwnGoal(PlayerController2D playerController, MatchManager matchManager)
    {
        int attackDirection = playerController.GetAttackDirection();

        if (attackDirection > 0)
        {
            if (matchManager.leftGoalTrigger == null)
            {
                return null;
            }

            return matchManager.leftGoalTrigger.transform;
        }

        if (matchManager.rightGoalTrigger == null)
        {
            return null;
        }

        return matchManager.rightGoalTrigger.transform;
    }

    private Sprite GetWhiteSquareSprite()
    {
        if (whiteSquareSprite != null)
        {
            return whiteSquareSprite;
        }

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        whiteSquareSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f
        );

        return whiteSquareSprite;
    }
}