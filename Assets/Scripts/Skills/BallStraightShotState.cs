using UnityEngine;

public class BallStraightShotState : MonoBehaviour
{
    private Rigidbody2D rb;

    private bool isStraightShotActive;
    private float originalGravityScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
        }
    }

    public void ActivateStraightShot(int direction, float speed)
    {
        if (rb == null)
        {
            return;
        }

        originalGravityScale = rb.gravityScale;
        isStraightShotActive = true;

        rb.gravityScale = 0f;
        rb.angularVelocity = 0f;
        rb.linearVelocity = new Vector2(direction * speed, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
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
        isStraightShotActive = false;
    }

    public void ForceRestoreNormalBall()
    {
        if (rb == null)
        {
            return;
        }

        rb.gravityScale = originalGravityScale;
        isStraightShotActive = false;
    }
}