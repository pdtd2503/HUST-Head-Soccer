using UnityEngine;

public class FallingObstacle :
MonoBehaviour
{
    bool landed;

    public float disappearTime = 5f;

    void OnCollisionEnter2D(
        Collision2D col
    )
    {
        if (landed)
            return;

        //--------------------------------
        // PLAYER
        //--------------------------------

        if (
            col.gameObject
            .CompareTag("Player")
        )
        {
            PlayerController2D player =
                col.gameObject
                .GetComponent<PlayerController2D>();

            if (player != null)
            {
                player.Stun(1f);
            }

            Destroy(gameObject);

            return;
        }

        //--------------------------------
        // BALL
        //--------------------------------

        if (
            col.gameObject
            .CompareTag("Ball")
        )
        {
            Rigidbody2D rb =
                col.gameObject
                .GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity =
                    new Vector2(
                        -rb.linearVelocity.x,
                        10
                    );
            }

            return;
        }

        //--------------------------------
        // GROUND
        //--------------------------------

        if (
            col.gameObject
            .CompareTag("Ground")
        )
        {
            Land();
        }
    }

    void Land()
    {
        landed = true;

        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        rb.bodyType =
            RigidbodyType2D.Static;

        Destroy(
            gameObject,
            disappearTime
        );
    }
}