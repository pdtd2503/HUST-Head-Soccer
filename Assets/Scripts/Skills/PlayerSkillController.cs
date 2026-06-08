using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Input")]
    public KeyCode skillKey = KeyCode.LeftShift;

    private const float SEEE_DASH_BALL_OFFSET = 0.75f;

    private const float SME_TARGET_Y = 6f;
    private const float SME_MIN_UP_SPEED = 6f;

    private const float SOICT_STRAIGHT_SHOT_SPEED = 18f;

    private const float SCLS_POISON_FORWARD_DISTANCE = 1f;
    private const float SCLS_POISON_WIDTH = 1f;
    private const float SCLS_POISON_HEIGHT = 1f;
    private const float SCLS_POISON_LIFE_TIME = 3f;
    private const float SCLS_STUN_DURATION = 1f;
    private const float SCLS_SPEED_BOOST_DURATION = 2f;

    private PlayerController2D playerController;
    private PlayerController2D opponentController;
    private MatchManager matchManager;

    private Transform ball;
    private Rigidbody2D ballRb;

    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();
        matchManager = FindFirstObjectByType<MatchManager>();

        GameObject ballObject = GameObject.FindGameObjectWithTag("Ball");

        if (ballObject != null)
        {
            ball = ballObject.transform;
            ballRb = ballObject.GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        FindOpponentController();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(skillKey))
        {
            return;
        }

        if (matchManager != null && !matchManager.CanUsePlayerActions())
        {
            return;
        }

        if (playerController == null || playerController.characterData == null)
        {
            return;
        }

        UseSkill(playerController.characterData.skillType);
    }

    private void FindOpponentController()
    {
        PlayerController2D[] players = FindObjectsByType<PlayerController2D>(
            FindObjectsSortMode.None
        );

        foreach (PlayerController2D player in players)
        {
            if (player != playerController)
            {
                opponentController = player;
                return;
            }
        }
    }

    private void UseSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.SEEE_DashToBall:
                UseSEEEDashToBall();
                break;

            case SkillType.SME_BallUpDoubleJump:
                UseSMEBallUpDoubleJump();
                break;

            case SkillType.SOICT_StraightShot:
                UseSOICTStraightShot();
                break;

            case SkillType.SCLS_Poison:
                UseSCLSPoison();
                break;
        }
    }

    private void UseSEEEDashToBall()
    {
        if (ball == null)
        {
            return;
        }

        int attackDirection = playerController.GetAttackDirection();

        Vector3 targetPosition = transform.position;

        targetPosition.x = ball.position.x - attackDirection * SEEE_DASH_BALL_OFFSET;
        targetPosition.y = ball.position.y;

        Rigidbody2D playerRb = playerController.GetBody();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        transform.position = targetPosition;
    }

    private void UseSMEBallUpDoubleJump()
    {
        if (ballRb != null)
        {
            float gravity = Mathf.Abs(Physics2D.gravity.y * ballRb.gravityScale);
            float height = Mathf.Max(0f, SME_TARGET_Y - ballRb.position.y);

            float requiredUpSpeed = Mathf.Sqrt(2f * gravity * height);
            requiredUpSpeed = Mathf.Max(requiredUpSpeed, SME_MIN_UP_SPEED);

            ballRb.linearVelocity = new Vector2(
                ballRb.linearVelocity.x * 0.2f,
                requiredUpSpeed
            );

            ballRb.angularVelocity = 0f;
        }

        playerController.EnableNextDoubleJump();
    }

    private void UseSOICTStraightShot()
    {
        if (ball == null)
        {
            return;
        }

        BallStraightShotState straightShotState =
            ball.GetComponent<BallStraightShotState>();

        if (straightShotState == null)
        {
            straightShotState =
                ball.gameObject.AddComponent<BallStraightShotState>();
        }

        int attackDirection = playerController.GetAttackDirection();

        straightShotState.ActivateStraightShot(
            attackDirection,
            SOICT_STRAIGHT_SHOT_SPEED
        );
    }

    private void UseSCLSPoison()
    {
        int attackDirection = playerController.GetAttackDirection();

        Vector3 spawnPosition = transform.position;
        spawnPosition.x += attackDirection * SCLS_POISON_FORWARD_DISTANCE;

        GameObject poisonObject = new GameObject("SCLS_PoisonField");
        poisonObject.transform.position = spawnPosition;
        poisonObject.transform.localScale =
            new Vector3(SCLS_POISON_WIDTH, SCLS_POISON_HEIGHT, 1f);

        SpriteRenderer spriteRenderer =
            poisonObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = CreateWhiteSquareSprite();
        spriteRenderer.color = new Color(0.2f, 1f, 0.2f, 0.6f);
        spriteRenderer.sortingOrder = 5;

        BoxCollider2D boxCollider =
            poisonObject.AddComponent<BoxCollider2D>();

        boxCollider.isTrigger = true;

        PoisonField poisonField =
            poisonObject.AddComponent<PoisonField>();

        poisonField.Setup(
            playerController,
            opponentController,
            SCLS_POISON_LIFE_TIME,
            SCLS_STUN_DURATION,
            SCLS_SPEED_BOOST_DURATION
        );
    }

    private Sprite CreateWhiteSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f
        );
    }
}