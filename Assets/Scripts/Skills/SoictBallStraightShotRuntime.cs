using UnityEngine;

public class SoictBallStraightShotRuntime : MonoBehaviour
{
    private Rigidbody2D rb;

    private bool isStraightShotActive;
    private float originalGravityScale;
    private RigidbodyType2D originalBodyType;
    private RigidbodyConstraints2D originalConstraints;
    private float straightShotTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            SaveOriginalPhysics();
        }
    }

    private void Update()
    {
        if (!isStraightShotActive)
        {
            return;
        }

        straightShotTimer -= Time.deltaTime;

        if (straightShotTimer <= 0f)
        {
            RestoreNormalBall();
        }
    }

    public bool IsStraightShotActive()
    {
        return isStraightShotActive;
    }
    
    public void ActivateStraightShot(int direction, float speed, float maxDuration)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rb == null)
        {
            return;
        }

        SaveOriginalPhysics();

        isStraightShotActive = true;
        straightShotTimer = maxDuration;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 0f;
        rb.angularVelocity = 0f;
        rb.linearVelocity = new Vector2(direction * speed, 0f);

        // THÊM MỚI: bật hiệu ứng binary comet trail
        BinaryCometTrail trail = GetComponent<BinaryCometTrail>();
        if (trail == null)
        {
            trail = gameObject.AddComponent<BinaryCometTrail>();
        }
        trail.enabled = true;
    }

    private void SaveOriginalPhysics()
    {
        originalGravityScale = rb.gravityScale;
        originalBodyType = rb.bodyType;
        originalConstraints = rb.constraints;
    }

        private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            return; // chạm đất thì không restore, giữ nguyên hiệu ứng
        }

        RestoreNormalBall();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            return;
        }

        RestoreNormalBall();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            return;
        }

        RestoreNormalBall();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            return;
        }

        RestoreNormalBall();
    }

    public void RestoreNormalBall()
    {
        if (rb == null)
        {
            return;
        }

        if (!isStraightShotActive)
        {
            return;
        }

        rb.gravityScale = originalGravityScale;
        rb.bodyType = originalBodyType;
        rb.constraints = originalConstraints;

        isStraightShotActive = false;
        straightShotTimer = 0f;

        // THÊM MỚI: tắt hiệu ứng trail
        BinaryCometTrail trail = GetComponent<BinaryCometTrail>();
        if (trail != null)
        {
            trail.enabled = false;
        }
    }

    public void ForceRestoreNormalBall()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rb == null)
        {
            return;
        }

        rb.gravityScale = originalGravityScale;
        rb.bodyType = originalBodyType;
        rb.constraints = originalConstraints;

        isStraightShotActive = false;
        straightShotTimer = 0f;
    }
}