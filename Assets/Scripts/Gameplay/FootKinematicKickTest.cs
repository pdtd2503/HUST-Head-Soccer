using UnityEngine;

public class FootKinematicKickTest : MonoBehaviour
{
    [Header("Input")]
    public KeyCode kickKey = KeyCode.F;

    [Header("Owner")]
    public Transform player;

    [Header("Direction")]
    public int kickDirection = 1; // Player 1 = 1, Player 2 = -1

    [Header("Local Motion")]
    public Vector2 startLocalPosition = new Vector2(-0.35f, -0.75f);
    public Vector2 endLocalPosition = new Vector2(0.15f, 0.10f);

    [Header("Rotation")]
    public float startAngle = -30f;
    public float endAngle = 90f;

    [Header("Speed")]
    public float swingTime = 0.12f;

    private Rigidbody2D rb;
    private float progress;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (player == null)
        {
            player = transform.parent;
        }

        ApplyPose(0f);
    }

    private void FixedUpdate()
    {
        float speed = 1f / swingTime;

        if (Input.GetKey(kickKey))
        {
            progress += speed * Time.fixedDeltaTime;
        }
        else
        {
            progress -= speed * Time.fixedDeltaTime;
        }

        progress = Mathf.Clamp01(progress);

        ApplyPose(progress);
    }

    private void ApplyPose(float t)
    {
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        Vector2 localStart = startLocalPosition;
        Vector2 localEnd = endLocalPosition;

        float angleStart = startAngle;
        float angleEnd = endAngle;

        if (kickDirection == -1)
        {
            localStart.x *= -1f;
            localEnd.x *= -1f;

            angleStart = 180f - startAngle;
            angleEnd = 180f - endAngle;
        }

        Vector2 localPosition = Vector2.Lerp(localStart, localEnd, smoothT);

        Vector3 worldPosition = player.TransformPoint(localPosition);

        float worldAngle = Mathf.LerpAngle(angleStart, angleEnd, smoothT);

        rb.MovePosition(worldPosition);
        rb.MoveRotation(worldAngle);
    }
}