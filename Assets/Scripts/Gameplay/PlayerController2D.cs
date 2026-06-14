using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData characterData;

    [Tooltip("Player 1 tấn công sang phải, Player 2 tấn công sang trái.")]
    public bool isPlayer1 = true;

    [Header("Character Visual")]
    public SpriteRenderer headRenderer;
    public SpriteRenderer footRenderer;

    [Header("Input")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;

    [Header("Jump Calibration")]
    [Tooltip("Vị trí Y của tâm player khi đang đứng trên mặt đất. Nếu đầu đường kính 1 và tâm ở y = 0.5 thì để 0.5.")]
    [SerializeField] private float standingCenterY = 0.5f;

    [Tooltip("Khoảng cách từ tâm player tới đỉnh đầu. Nếu đầu đường kính 1 thì để 0.5.")]
    [SerializeField] private float centerToHeadTop = 0.5f;

    [Header("Runtime Debug")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private bool isGrounded;

    private Rigidbody2D rb;
    private float moveInput;
    private float baseMoveSpeed;

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
        ApplyMovement();
    }

    public void ApplyCharacterData()
    {
        if (characterData == null)
        {
            Debug.LogWarning($"{name} has no CharacterData assigned.");
            return;
        }

        ApplyCharacterVisual();
        ApplySpeedFromStars(characterData.speedStars);
        ApplyJumpFromStars(characterData.jumpStars);
        ApplyMassFromStars(characterData.massStars);

        Debug.Log(
            $"{name} applied: " +
            $"Speed={moveSpeed}, " +
            $"JumpVelocity={jumpVelocity}, " +
            $"Mass={rb.mass}"
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

    private void ApplySpeedFromStars(int speedStars)
    {
        baseMoveSpeed = CharacterStats.GetSpeed(speedStars);
        moveSpeed = baseMoveSpeed;
    }

    private void ApplyJumpFromStars(int jumpStars)
    {
        float headReach = CharacterStats.GetJumpHeadReach(jumpStars);

        float targetCenterY = headReach - centerToHeadTop;
        float jumpHeight = targetCenterY - standingCenterY;

        if (jumpHeight <= 0f)
        {
            jumpVelocity = 0f;
            return;
        }

        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        jumpVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
    }

    private void ApplyMassFromStars(int massStars)
    {
        rb.mass = CharacterStats.GetMass(massStars);
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

    private void ApplyMovement()
    {
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void ReadJumpInput()
    {
        if (!Input.GetKeyDown(jumpKey))
        {
            return;
        }

        if (!isGrounded)
        {
            return;
        }

        Jump();
        isGrounded = false;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpVelocity
        );
    }

    private void OnCollisionStay2D(Collision2D collision)
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public int GetAttackDirection()
    {
        if (isPlayer1)
        {
            return 1;
        }

        return -1;
    }

    public Rigidbody2D GetBody()
    {
        return rb;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetJumpVelocity()
    {
        return jumpVelocity;
    }

    public float GetMass()
    {
        return rb.mass;
    }
}