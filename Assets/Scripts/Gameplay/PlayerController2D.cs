using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData characterData;

    [Header("Input")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;

    [Header("Runtime Tuning")]
    public float moveMultiplier = 4f;
    public float jumpForceMultiplier = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool isGrounded;
    private float moveInput;

    private float moveSpeed;
    private float jumpForce;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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

        if (sr != null && characterData.characterSprite != null)
        {
            sr.sprite = characterData.characterSprite;
        }

        float designSpeed =
            CharacterStats.GetSpeed(characterData.speedStars);

        float jumpReach =
            CharacterStats.GetJumpReach(characterData.jumpStars);

        float mass =
            CharacterStats.GetMass(characterData.massStars);

        moveSpeed = designSpeed * moveMultiplier;

        float jumpCenterHeight = jumpReach - 1f;

        float gravity =
            Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        jumpForce =
            Mathf.Sqrt(2f * gravity * jumpCenterHeight)
            * jumpForceMultiplier;

        rb.mass = mass;

        Debug.Log(
            $"{name} applied: Speed={moveSpeed}, Jump={jumpForce}, Mass={mass}");
    }

    void ReadMoveInput()
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

    void ReadJumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.linearVelocity =
                new Vector2(rb.linearVelocity.x, jumpForce);

            isGrounded = false;
        }
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
}