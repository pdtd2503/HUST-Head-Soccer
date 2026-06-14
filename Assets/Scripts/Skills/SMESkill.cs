using UnityEngine;

public static class SMESkill
{
    private const float WALL_DURATION = 3f;
    private const float WALL_WIDTH = 0.25f;
    private const float WALL_IN_FIELD_OFFSET = 0.15f;

    private static Sprite whiteSquareSprite;

    public static void UseSkill(
        PlayerController2D playerController,
        MatchManager matchManager
    )
    {
        if (playerController == null)
        {
            Debug.LogWarning("SME skill failed: missing playerController.");
            return;
        }

        if (matchManager == null)
        {
            Debug.LogWarning("SME skill failed: missing matchManager.");
            return;
        }

        GoalTrigger ownGoalTrigger =
            GetOwnGoalTrigger(playerController, matchManager);

        if (ownGoalTrigger == null)
        {
            Debug.LogWarning("SME skill failed: could not find own goal trigger.");
            return;
        }

        BoxCollider2D goalCollider =
            ownGoalTrigger.GetComponent<BoxCollider2D>();

        if (goalCollider == null)
        {
            Debug.LogWarning("SME skill failed: own goal trigger has no BoxCollider2D.");
            return;
        }

        Bounds goalBounds = goalCollider.bounds;

        int attackDirection = playerController.GetAttackDirection();

        Vector3 wallPosition = goalBounds.center;

        if (attackDirection > 0)
        {
            // Player 1 tấn công sang phải, gôn nhà là bên trái.
            // Tường đặt ở mép trong của gôn trái, hơi nhô vào sân.
            wallPosition.x = goalBounds.max.x + WALL_IN_FIELD_OFFSET;
        }
        else
        {
            // Player 2 tấn công sang trái, gôn nhà là bên phải.
            // Tường đặt ở mép trong của gôn phải, hơi nhô vào sân.
            wallPosition.x = goalBounds.min.x - WALL_IN_FIELD_OFFSET;
        }

        CreateWall(wallPosition, goalBounds.size.y);

        Debug.Log($"{playerController.name} used SME wall skill.");
    }

    private static GoalTrigger GetOwnGoalTrigger(
        PlayerController2D playerController,
        MatchManager matchManager
    )
    {
        int attackDirection = playerController.GetAttackDirection();

        if (attackDirection > 0)
        {
            return matchManager.leftGoalTrigger;
        }

        return matchManager.rightGoalTrigger;
    }

    private static void CreateWall(Vector3 position, float height)
    {
        GameObject wall = new GameObject("SME_GoalWall");

        wall.transform.position = position;
        wall.transform.rotation = Quaternion.identity;

        // Kích thước nhìn thấy của tường trong world unit
        wall.transform.localScale =
            new Vector3(WALL_WIDTH, height, 1f);

        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetWhiteSquareSprite();
        spriteRenderer.color = new Color(0.55f, 0.55f, 0.55f, 0.85f);
        spriteRenderer.sortingOrder = 50;

        BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
        wallCollider.isTrigger = false;

        // Vì transform.localScale đã quyết định kích thước thật,
        // collider để size 1x1 để khớp với sprite.
        wallCollider.size = Vector2.one;

        Rigidbody2D wallRb = wall.AddComponent<Rigidbody2D>();
        wallRb.bodyType = RigidbodyType2D.Static;

        int wallLayer = LayerMask.NameToLayer("Wall");

        if (wallLayer >= 0)
        {
            wall.layer = wallLayer;
        }

        Object.Destroy(wall, WALL_DURATION);
    }

    private static Sprite GetWhiteSquareSprite()
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