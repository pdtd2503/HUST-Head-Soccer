using UnityEngine;

public static class SOICTSkill
{
    private const float STRAIGHT_SHOT_SPEED = 22f;
    private const float MAX_STRAIGHT_SHOT_TIME = 1.5f;

    public static void UseSkill(
        PlayerController2D playerController,
        Rigidbody2D ballRb
    )
    {
        if (playerController == null || ballRb == null)
        {
            return;
        }

        SoictBallStraightShotRuntime runtime =
            ballRb.GetComponent<SoictBallStraightShotRuntime>();

        if (runtime == null)
        {
            runtime =
                ballRb.gameObject.AddComponent<SoictBallStraightShotRuntime>();
        }

        int attackDirection = playerController.GetAttackDirection();

        runtime.ActivateStraightShot(
            attackDirection,
            STRAIGHT_SHOT_SPEED,
            MAX_STRAIGHT_SHOT_TIME
        );
    }
}

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

    public void ActivateStraightShot(
        int direction,
        float speed,
        float maxDuration
    )
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
    }

    private void SaveOriginalPhysics()
    {
        originalGravityScale = rb.gravityScale;
        originalBodyType = rb.bodyType;
        originalConstraints = rb.constraints;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isStraightShotActive)
        {
            return;
        }

        RestoreNormalBall();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isStraightShotActive)
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