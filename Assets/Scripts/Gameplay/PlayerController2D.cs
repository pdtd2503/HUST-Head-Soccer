using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData characterData;

    [Tooltip("Player 1 tấn công sang phải, Player 2 tấn công sang trái")]
    public bool isPlayer1 = true;

    [Header("Character Visual")]
    public SpriteRenderer headRenderer;
    public SpriteRenderer footRenderer;

    [Header("Input")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;

    [Header("Runtime Tuning")]
    public float moveMultiplier = 1f;
    public float jumpForceMultiplier = 1f;

    [Header("Ball Contact Fix")]
    [Tooltip("Chặn player bị bóng đẩy nảy lên khi player đang ở phía trên bóng.")]
    [SerializeField] private bool preventBounceFromBall = true;

    [Tooltip("Tag của quả bóng trong scene.")]
    [SerializeField] private string ballTag = "Ball";

    [Tooltip("Contact normal càng gần 1 thì càng chắc chắn bóng đang ở dưới player.")]
    [SerializeField] private float ballTopContactNormalY = 0.5f;

    [Tooltip("Vận tốc Y dương tối đa được giữ lại khi player chạm phía trên bóng.")]
    [SerializeField] private float maxUpwardSpeedFromBall = 0f;

    [Header("Anti Ball Launch")]
    [Tooltip("Giới hạn vận tốc bay lên khi player bị bóng hất sau va chạm mạnh.")]
    [SerializeField] private bool limitBallLaunch = true;

    [Tooltip("Vận tốc Y dương tối đa khi player bị bóng hất lên. Không nên để 0 vì sẽ làm cảm giác quá cứng.")]
    [SerializeField] private float maxBallLaunchUpSpeed = 1.5f;

    [Tooltip("Sau khi player tự bấm nhảy, bỏ qua chống hất trong khoảng thời gian này để cú nhảy vẫn tự nhiên.")]
    [SerializeField] private float jumpIgnoreDuration = 0.25f;

    [Header("Player Collision Push")]
    [SerializeField] private bool useMassStarPush = true;

    [Tooltip("Chênh 1 sao mass thì player nhẹ hơn bị đẩy thêm bao nhiêu unit/s.")]
    [SerializeField] private float pushSpeedPerMassStar = 2f;

    [Tooltip("Thời gian giữ lực đẩy sau va chạm player-player.")]
    [SerializeField] private float pushDuration = 0.08f;

    private Rigidbody2D rb;

    private bool isGrounded;
    private float moveInput;
    private MatchManager matchManager;
    private bool isStunned = false;

    private float moveSpeed;
    private float baseMoveSpeed;
    private float jumpForce;

    private float lastJumpTime = -999f;
    private Coroutine speedBoostCoroutine;

    private float playerPushVelocityX;
    private float playerPushTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        matchManager = FindFirstObjectByType<MatchManager>();
    }

    private void Start()
    {
        ApplyCharacterData();
    }

    private void Update()
    {
        if (!CanAct())
        {
            moveInput = 0f;
            return;
        }

        if (isStunned)
        {
            moveInput = 0f;
            return;
        }

        ReadMoveInput();
        ReadJumpInput();
    }

    private void FixedUpdate()
    {
        if (!CanAct())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isStunned)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        UpdatePlayerPushTimer();

        float targetVelocityX =
            moveInput * moveSpeed + playerPushVelocityX;

        rb.linearVelocity =
            new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    private bool CanAct()
    {
        return matchManager == null || matchManager.CanUsePlayerActions();
    }

    private void UpdatePlayerPushTimer()
    {
        if (playerPushTimer > 0f)
        {
            playerPushTimer -= Time.fixedDeltaTime;
        }
        else
        {
            playerPushVelocityX = 0f;
        }
    }

    public void ApplyCharacterData()
    {
        if (characterData == null)
        {
            Debug.LogWarning($"{name} has no CharacterData assigned.");
            return;
        }

        ApplyCharacterVisual();

        float designSpeed =
            CharacterStats.GetSpeed(characterData.speedStars);

        float jumpReach =
            CharacterStats.GetJumpReach(characterData.jumpStars);

        float mass =
            CharacterStats.GetMass(characterData.massStars);

        moveSpeed = designSpeed * moveMultiplier;
        baseMoveSpeed = moveSpeed;

        float jumpCenterHeight = jumpReach - 1f;
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        jumpForce =
            Mathf.Sqrt(2f * gravity * jumpCenterHeight)
            * jumpForceMultiplier;

        rb.mass = mass;

        Debug.Log(
            $"{name} applied: Speed={moveSpeed}, Jump={jumpForce}, Mass={mass}"
        );
    }

    private void ApplyCharacterVisual()
    {
        if (characterData == null)
        {
            return;
        }

        if (isPlayer1)
        {
            if (headRenderer != null)
            {
                headRenderer.sprite = characterData.headRightSprite;
            }

            if (footRenderer != null)
            {
                footRenderer.sprite = characterData.footRightSprite;
            }
        }
        else
        {
            if (headRenderer != null)
            {
                headRenderer.sprite = characterData.headLeftSprite;
            }

            if (footRenderer != null)
            {
                footRenderer.sprite = characterData.footLeftSprite;
            }
        }
    }

    private void ReadMoveInput()
    {
        moveInput = 0f;

        if (Input.GetKey(leftKey))
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(rightKey))
        {
            moveInput = 1f;
        }
    }

    private void ReadJumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.linearVelocity =
                new Vector2(rb.linearVelocity.x, jumpForce);

            lastJumpTime = Time.time;
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGroundContact(collision);
        HandleBallCollision(collision);
        HandlePlayerMassPush(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGroundContact(collision);
        HandleBallCollision(collision);
        HandlePlayerMassPush(collision);
    }

    private void CheckGroundContact(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Ground"))
        {
            return;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);

            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void HandleBallCollision(Collision2D collision)
    {
        if (!collision.collider.CompareTag(ballTag))
        {
            return;
        }

        PreventBounceFromBall(collision);
        LimitBallLaunch(collision);
    }

    private void PreventBounceFromBall(Collision2D collision)
    {
        if (!preventBounceFromBall)
        {
            return;
        }

        bool justJumped =
            Time.time - lastJumpTime <= jumpIgnoreDuration;

        if (justJumped)
        {
            return;
        }

        bool playerIsAboveBall =
            transform.position.y > collision.transform.position.y;

        if (!playerIsAboveBall)
        {
            return;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);

            if (contact.normal.y > ballTopContactNormalY &&
                rb.linearVelocity.y > maxUpwardSpeedFromBall)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    maxUpwardSpeedFromBall
                );

                return;
            }
        }
    }

    private void LimitBallLaunch(Collision2D collision)
    {
        if (!limitBallLaunch)
        {
            return;
        }

        bool justJumped =
            Time.time - lastJumpTime <= jumpIgnoreDuration;

        if (justJumped)
        {
            return;
        }

        if (rb.linearVelocity.y > maxBallLaunchUpSpeed)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                maxBallLaunchUpSpeed
            );
        }
    }

    private void HandlePlayerMassPush(Collision2D collision)
    {
        if (!useMassStarPush)
        {
            return;
        }

        PlayerController2D otherPlayer =
            collision.collider.GetComponentInParent<PlayerController2D>();

        if (otherPlayer == null || otherPlayer == this)
        {
            return;
        }

        if (characterData == null || otherPlayer.characterData == null)
        {
            return;
        }

        int myMassStars = characterData.massStars;
        int otherMassStars = otherPlayer.characterData.massStars;

        int starDifference = otherMassStars - myMassStars;

        if (starDifference <= 0)
        {
            return;
        }

        float pushDirection;

        if (transform.position.x >= otherPlayer.transform.position.x)
        {
            pushDirection = 1f;
        }
        else
        {
            pushDirection = -1f;
        }

        playerPushVelocityX =
            pushDirection * starDifference * pushSpeedPerMassStar;

        playerPushTimer = pushDuration;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public Rigidbody2D GetBody()
    {
        return rb;
    }

    public int GetAttackDirection()
    {
        return isPlayer1 ? 1 : -1;
    }

    public int GetMassStars()
    {
        if (characterData == null)
        {
            return 3;
        }

        return characterData.massStars;
    }

    public void Stun(float duration)
    {
        StopAllCoroutines();

        StartCoroutine(
            StunRoutine(duration)
        );
    }

    private System.Collections.IEnumerator StunRoutine(float duration)
    {
        isStunned = true;

        rb.linearVelocity =
            Vector2.zero;

        yield return new WaitForSeconds(
            duration
        );

        isStunned = false;
    }

    public void SetTemporarySpeedStars(int stars, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(TemporarySpeedRoutine(stars, duration));
    }

    private System.Collections.IEnumerator TemporarySpeedRoutine(int stars, float duration)
    {
        moveSpeed = CharacterStats.GetSpeed(stars) * moveMultiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = baseMoveSpeed;
        speedBoostCoroutine = null;
    }
}