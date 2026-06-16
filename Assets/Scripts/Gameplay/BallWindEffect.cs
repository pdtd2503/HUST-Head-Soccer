using UnityEngine;

public class BallWindEffect : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(
            Vector2.right
            * WindManager.CurrentWind
        );
    }
}