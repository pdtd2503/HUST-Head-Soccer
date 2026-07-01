using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    bool landed;

    public float disappearTime = 5f;

    [Header("Stun Effect")]
    [HideInInspector] public GameObject starPrefab;
    [SerializeField] private int starCount = 3;
    [SerializeField] private float orbitRadius = 0.5f;
    [SerializeField] private float orbitSpeed = 180f;
    [SerializeField] private float starHeight = 1.2f;
    [HideInInspector] public AudioClip hitSound; // truyền từ RainSpawner
    void OnCollisionEnter2D(Collision2D col)
    {
        if (landed) return;

        // PLAYER
        if (col.gameObject.CompareTag("Player"))
    {
    AudioManager.Instance?.PlayObstacleHit(); 
    Debug.Log("FallingObstacle: Chạm player!");
    
    PlayerController2D player = col.gameObject.GetComponent<PlayerController2D>();

        if (player != null)
        {
            Debug.Log("FallingObstacle: Tìm thấy PlayerController2D!");
            
            player.Stun(0.75f);

            StunEffectRunner runner = player.GetComponent<StunEffectRunner>();
            if (runner == null)
            {
                runner = player.gameObject.AddComponent<StunEffectRunner>();
                Debug.Log("FallingObstacle: Tạo StunEffectRunner mới!");
            }

            Debug.Log($"FallingObstacle: starPrefab = {starPrefab}");

            runner.PlayStunEffect(
                player.transform,
                0.75f,
                starPrefab,
                starCount,
                orbitRadius,
                orbitSpeed,
                starHeight
            );
        }
        else
        {
            Debug.LogWarning("FallingObstacle: Không tìm thấy PlayerController2D!");
        }

        Destroy(gameObject);
        return;
    }

        // BALL
        if (col.gameObject.CompareTag("Ball"))
        {
            AudioManager.Instance?.PlayObstacleHit();
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = new Vector2(-rb.linearVelocity.x, 10);
            }

            return;
        }

        // GROUND
        if (col.gameObject.CompareTag("Ground"))
        {
            AudioManager.Instance?.PlayObstacleHit();
            Land();
        }
    }

    void Land()
    {
        landed = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        Destroy(gameObject, disappearTime);
    }
}