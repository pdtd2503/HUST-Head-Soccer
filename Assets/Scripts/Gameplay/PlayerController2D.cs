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

    [Tooltip("Vận tốc Y dương tối đa được giữ lại khi player chạm phía trên bóng. 0 = không cho bóng hất player lên.")]
    [SerializeField] private float maxUpwardSpeedFromBall = 0f;

    private Rigidbody2D rb;

    private bool isGrounded;
    private float moveInput;

    private float moveSpeed;
    private float jumpForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ApplyCharacterData();
    }

    private void Update()
    {
        ReadMoveInput();
        ReadJumpInput();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity =
            new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
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

            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PreventBounceFromBall(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGroundContact(collision);
        PreventBounceFromBall(collision);
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

    private void PreventBounceFromBall(Collision2D collision)
    {
        if (!preventBounceFromBall)
        {
            return;
        }

        if (!collision.collider.CompareTag(ballTag))
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
}