using System.Collections;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData characterData;

    [Tooltip("Player 1 dùng sprite quay sang phải, Player 2 dùng sprite quay sang trái")]
    public bool isPlayer1 = true;

    [Header("Character Visual")]
    public SpriteRenderer headRenderer;
    public SpriteRenderer footRenderer;

    [Header("Input")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;

    [Header("Runtime Tuning")]
    public float moveMultiplier = 4f;
    public float jumpForceMultiplier = 5f;

    private Rigidbody2D rb;

    private bool isGrounded;
    private float moveInput;

    private float moveSpeed;
    private float baseMoveSpeed;
    private float jumpForce;

    private bool isStunned;

    private bool nextJumpHasDoubleJump;
    private int extraJumpsRemaining;

    private Coroutine stunCoroutine;
    private Coroutine speedBoostCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        ApplyCharacterData();
    }

    void Update()
    {
        ReadMoveInput();
        ReadJumpInput();
    }

    void FixedUpdate()
    {
        if (isStunned)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    public void ApplyCharacterData()
    {
        if (characterData == null)
        {
            Debug.LogWarning($"{name} has no CharacterData assigned.");
            return;
        }

        ApplyCharacterVisual();

        float designSpeed = CharacterStats.GetSpeed(characterData.speedStars);
        float jumpReach = CharacterStats.GetJumpReach(characterData.jumpStars);
        float mass = CharacterStats.GetMass(characterData.massStars);

        baseMoveSpeed = designSpeed * moveMultiplier;
        moveSpeed = baseMoveSpeed;

        float jumpCenterHeight = jumpReach - 1f;
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        jumpForce = Mathf.Sqrt(2f * gravity * jumpCenterHeight) * jumpForceMultiplier;

        rb.mass = mass;

        Debug.Log($"{name} applied: Speed={moveSpeed}, Jump={jumpForce}, Mass={mass}");
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

    void ReadMoveInput()
    {
        if (isStunned)
        {
            moveInput = 0f;
            return;
        }

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

    void ReadJumpInput()
    {
        if (isStunned)
        {
            return;
        }

        if (!Input.GetKeyDown(jumpKey))
        {
            return;
        }

        if (isGrounded)
        {
            Jump();

            if (nextJumpHasDoubleJump)
            {
                extraJumpsRemaining = 1;
                nextJumpHasDoubleJump = false;
            }

            isGrounded = false;
            return;
        }

        if (extraJumpsRemaining > 0)
        {
            Jump();
            extraJumpsRemaining--;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void OnCollisionStay2D(Collision2D collision)
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

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void EnableNextDoubleJump()
    {
        nextJumpHasDoubleJump = true;
        extraJumpsRemaining = 0;
    }

    public void Stun(float duration)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        moveInput = 0f;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        yield return new WaitForSeconds(duration);

        isStunned = false;
        stunCoroutine = null;
    }

    public void SetTemporarySpeedStars(int stars, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(TemporarySpeedRoutine(stars, duration));
    }

    private IEnumerator TemporarySpeedRoutine(int stars, float duration)
    {
        moveSpeed = CharacterStats.GetSpeed(stars) * moveMultiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = baseMoveSpeed;
        speedBoostCoroutine = null;
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