using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallWind : MonoBehaviour
{
    public float windMultiplier = 0.3f;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(
            Vector2.right *
            WindManager.CurrentWind *
            windMultiplier,
            ForceMode2D.Force
        );
    }
}