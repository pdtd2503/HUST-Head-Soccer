using UnityEngine;

public class GoalMover : MonoBehaviour
{
    public float moveDistance = 2f;
    public float moveSpeed = 2f;

    // Goal trái = 1
    // Goal phải = -1
    public float directionMultiplier = 1f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float yOffset =
            Mathf.Sin(Time.time * moveSpeed)
            * moveDistance
            * directionMultiplier;

        transform.position =
            startPos + Vector3.up * yOffset;
    }
}