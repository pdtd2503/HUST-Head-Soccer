using UnityEngine;

public class BallAudio : MonoBehaviour
{
    [SerializeField] private float minVelocity = 1f;   // va chạm yếu hơn này thì bỏ qua
    [SerializeField] private float maxVelocity = 15f;  // va chạm mạnh nhất (volume = 1)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (AudioManager.Instance == null) return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        // Bỏ qua va chạm quá nhẹ (tránh spam âm thanh khi bóng lăn chậm trên đất)
        if (impactSpeed < minVelocity) return;

        // Tính volume theo hàm toán học
        float volume = CalculateVolume(impactSpeed);

        if (collision.collider.GetComponentInParent<PlayerController2D>() != null)
        {
            AudioManager.Instance.PlayBallHitPlayer(volume);
        }
        else
        {
            AudioManager.Instance.PlayBallHitObject(volume);
        }
    }

    private float CalculateVolume(float speed)
    {
        // Dùng hàm căn bậc hai: tăng nhanh lúc đầu, chậm dần khi mạnh
        // Cho cảm giác tự nhiên hơn hàm tuyến tính
        float normalized = Mathf.Clamp01((speed - minVelocity) / (maxVelocity - minVelocity));
        return Mathf.Sqrt(normalized);
    }
}