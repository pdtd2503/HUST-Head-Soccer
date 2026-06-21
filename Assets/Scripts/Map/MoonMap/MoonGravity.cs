using UnityEngine;

public class MoonGravitySetup : MonoBehaviour
{
    [SerializeField] private float moonGravityScale = 0.5f;

    private void Start()
    {
        Invoke(nameof(ApplyMoonGravity), 0.1f); // chờ 1 chút để chắc chắn chạy SAU PlayerController2D
    }

    private void ApplyMoonGravity()
    {
        PlayerController2D[] players = FindObjectsByType<PlayerController2D>(FindObjectsSortMode.None);

        foreach (PlayerController2D player in players)
        {
            Rigidbody2D rb = player.GetBody();
            if (rb != null)
            {
                rb.gravityScale = moonGravityScale;
            }
        }
    }
}