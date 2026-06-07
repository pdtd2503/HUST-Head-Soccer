using UnityEngine;

public enum GoalSide
{
    Left,
    Right
}

public class GoalTrigger : MonoBehaviour
{
    public MatchManager matchManager;
    public GoalSide goalSide;
    public int scoringPlayer = 1;

    private bool goalLocked;
    private BoxCollider2D goalCollider;

    private void Awake()
    {
        goalCollider = GetComponent<BoxCollider2D>();

        if (goalCollider == null)
        {
            Debug.LogError($"{name} missing BoxCollider2D.");
        }
        else if (!goalCollider.isTrigger)
        {
            Debug.LogWarning($"{name} BoxCollider2D should be Is Trigger.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryScore(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryScore(other);
    }

    private void TryScore(Collider2D other)
    {
        if (goalLocked)
        {
            return;
        }

        if (!other.CompareTag("Ball"))
        {
            return;
        }

        if (matchManager == null)
        {
            Debug.LogError($"{name} missing MatchManager.");
            return;
        }

        if (!matchManager.CanScore())
        {
            return;
        }

        if (goalCollider == null)
        {
            return;
        }

        Bounds ballBounds = other.bounds;
        Bounds goalBounds = goalCollider.bounds;

        bool isGoal = false;

        if (goalSide == GoalSide.Left)
        {
            // Với gôn trái, bóng phải đi hoàn toàn qua mép trong bên phải của trigger.
            isGoal = ballBounds.max.x <= goalBounds.max.x;
        }
        else if (goalSide == GoalSide.Right)
        {
            // Với gôn phải, bóng phải đi hoàn toàn qua mép trong bên trái của trigger.
            isGoal = ballBounds.min.x >= goalBounds.min.x;
        }

        if (!isGoal)
        {
            return;
        }

        goalLocked = true;
        matchManager.ScoreGoal(scoringPlayer);
    }

    public void UnlockGoal()
    {
        goalLocked = false;
    }
}