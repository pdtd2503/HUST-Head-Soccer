using UnityEngine;
using System.Collections;

public class FootKick : MonoBehaviour
{
    public KeyCode kickKey = KeyCode.F;

    // Player 1 = 1
    // Player 2 = -1
    public int kickDirection = 1;

    public float kickForce = 10f;
    public float kickHeight = 3f;

    private Vector3 startPos;
    private bool kicking;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(kickKey) && !kicking)
        {
            StartCoroutine(Kick());
        }
    }

    IEnumerator Kick()
    {
        kicking = true;

        // Chân thò ra
        transform.localPosition =
            startPos + Vector3.right * (0.5f * kickDirection);

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Ball"))
            {
                Rigidbody2D ballRb =
                    hit.GetComponent<Rigidbody2D>();

                if (ballRb != null)
                {
                    ballRb.AddForce(
                        new Vector2(
                            kickForce * kickDirection,
                            kickHeight
                        ),
                        ForceMode2D.Impulse
                    );
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        // Chân thu về
        transform.localPosition = startPos;

        kicking = false;
    }
}