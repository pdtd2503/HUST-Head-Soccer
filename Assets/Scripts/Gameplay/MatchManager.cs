using System.Collections;
using UnityEngine;
using TMPro;

public class MatchManager : MonoBehaviour
{
    [Header("Match Settings")]
    public float halfDuration = 45f;
    public int totalHalves = 2;
    public float goalPauseDuration = 3f;

    [Header("Players")]
    public Transform player1;
    public Transform player2;
    public Transform player1Spawn;
    public Transform player2Spawn;

    [Header("Ball")]
    public Transform ball;
    public Rigidbody2D ballRb;
    public Transform ballSpawn;

    [Header("Goal Triggers")]
    public GoalTrigger leftGoalTrigger;
    public GoalTrigger rightGoalTrigger;

    [Header("UI")]
    public TMP_Text goalText;

    private int currentHalf = 1;
    private float timer;

    private int player1Score;
    private int player2Score;

    private bool matchRunning = true;
    private bool goalSequenceRunning;

    private void Start()
    {
        timer = halfDuration;

        if (goalText != null)
        {
            goalText.gameObject.SetActive(false);
        }

        ResetPositions();
        Debug.Log("Match started. Half 1.");
    }

    private void Update()
    {
        if (!matchRunning || goalSequenceRunning)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EndHalf();
        }
    }

    private void EndHalf()
    {
        if (currentHalf < totalHalves)
        {
            currentHalf++;
            timer = halfDuration;
            ResetPositions();

            Debug.Log($"Half {currentHalf} started.");
        }
        else
        {
            matchRunning = false;
            timer = 0f;

            Debug.Log($"Match ended. Final Score: Player 1 {player1Score} - {player2Score} Player 2");
        }
    }

    public bool CanScore()
    {
        return matchRunning && !goalSequenceRunning;
    }

    public void ScoreGoal(int scoringPlayer)
    {
        if (!CanScore())
        {
            return;
        }

        StartCoroutine(GoalSequence(scoringPlayer));
    }

    private IEnumerator GoalSequence(int scoringPlayer)
    {
        goalSequenceRunning = true;

        if (scoringPlayer == 1)
        {
            player1Score++;
        }
        else if (scoringPlayer == 2)
        {
            player2Score++;
        }

        Debug.Log($"Player {scoringPlayer} scored. Score: {player1Score} - {player2Score}");

        ShowGoalText(scoringPlayer);

        StopAllBodies();

        yield return new WaitForSeconds(goalPauseDuration);

        HideGoalText();

        ResetPositions();

        UnlockGoalTriggers();

        goalSequenceRunning = false;
    }

    private void ShowGoalText(int scoringPlayer)
    {
        if (goalText == null)
        {
            return;
        }

        goalText.text = $"GOALLLL!\nPlayer {scoringPlayer} scored!";
        goalText.gameObject.SetActive(true);
    }

    private void HideGoalText()
    {
        if (goalText == null)
        {
            return;
        }

        goalText.gameObject.SetActive(false);
    }

    private void StopAllBodies()
    {
        StopBody(player1);
        StopBody(player2);

        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
    }

    private void ResetPositions()
    {
        ResetBody(player1, player1Spawn);
        ResetBody(player2, player2Spawn);

        if (ball != null && ballSpawn != null)
        {
            ball.position = ballSpawn.position;
        }

        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
    }

    private void ResetBody(Transform target, Transform spawn)
    {
        if (target == null || spawn == null)
        {
            return;
        }

        target.position = spawn.position;

        StopBody(target);
    }

    private void StopBody(Transform target)
    {
        if (target == null)
        {
            return;
        }

        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void UnlockGoalTriggers()
    {
        if (leftGoalTrigger != null)
        {
            leftGoalTrigger.UnlockGoal();
        }

        if (rightGoalTrigger != null)
        {
            rightGoalTrigger.UnlockGoal();
        }
    }

    public float GetTimer()
    {
        return timer;
    }

    public int GetCurrentHalf()
    {
        return currentHalf;
    }

    public int GetPlayer1Score()
    {
        return player1Score;
    }

    public int GetPlayer2Score()
    {
        return player2Score;
    }
}