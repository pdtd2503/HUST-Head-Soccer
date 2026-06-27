using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour
{
    [Header("Match Settings")]
    public float matchDuration = 90f;
    public float goalPauseDuration = 3f;

    [Header("Result Scene")]
    public string matchResultSceneName = "MatchResultScene";
    public float resultSceneDelay = 2f;

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

    private float timer;

    private int player1Score;
    private int player2Score;

    private bool matchRunning = true;
    private bool matchEnded;
    private bool goalSequenceRunning;
    private bool suddenDeathActive;

    private void Start()
    {
        ResolveRuntimeReferences();

        timer = 0f;
        matchRunning = true;
        matchEnded = false;
        goalSequenceRunning = false;

        if (goalText != null)
        {
            goalText.gameObject.SetActive(false);
        }

        ResetPositions();

        AudioManager.Instance?.PlayWhistleStart();

        Debug.Log("Match started. Duration: 90 seconds.");
    }

    private void Update()
    {
        if (!matchRunning || matchEnded || goalSequenceRunning)
        {
            return;
        }

        if (IsGoldenGoalMatch())
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= matchDuration)
        {
            timer = matchDuration;

            if (IsTournamentMatch() && player1Score == player2Score)
            {
                StartSuddenDeath();
            }
            else
            {
                EndMatch();
            }
        }
    }

    private void ResolveRuntimeReferences()
    {
        if (player1 == null || player2 == null)
        {
            PlayerController2D[] players =
                FindObjectsByType<PlayerController2D>(
                    FindObjectsSortMode.None
                );

            foreach (PlayerController2D player in players)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.isPlayer1 && player1 == null)
                {
                    player1 = player.transform;
                }
                else if (!player.isPlayer1 && player2 == null)
                {
                    player2 = player.transform;
                }
            }
        }

        if (ball == null)
        {
            GameObject ballObject = GameObject.FindGameObjectWithTag("Ball");

            if (ballObject != null)
            {
                ball = ballObject.transform;
            }
        }

        if (ballRb == null && ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody2D>();
        }

        if (leftGoalTrigger != null)
        {
            leftGoalTrigger.matchManager = this;
        }

        if (rightGoalTrigger != null)
        {
            rightGoalTrigger.matchManager = this;
        }
    }

    private void EndMatch()
    {
        if (matchEnded)
        {
            return;
        }

        matchRunning = false;
        matchEnded = true;
        goalSequenceRunning = false;

        StopAllCoroutines();

        StopAllBodies();
        FreezeAllBodies();

        UnlockGoalTriggers();

        HideGoalText();

        AudioManager.Instance?.PlayWhistleEnd(); 

        SaveMatchResult();

        Debug.Log(
            $"Match ended. Final Score: Player 1 {player1Score} - {player2Score} Player 2"
        );

        StartCoroutine(LoadResultSceneAfterDelay());
    }

    private void ShowFullTimeText()
    {
        if (goalText == null)
        {
            return;
        }

        goalText.text =
            $"HẾT GIỜ!\nPlayer 1 {player1Score} - {player2Score} Player 2";

        goalText.gameObject.SetActive(true);
    }

    public bool CanScore()
    {
        return matchRunning && !matchEnded && !goalSequenceRunning;
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

        Debug.Log(
            $"Player {scoringPlayer} scored. Score: {player1Score} - {player2Score}"
        );

        ShowGoalText(scoringPlayer);

        AudioManager.Instance?.PlayGoalCheer();

        StopAllBodies();

        if (IsGoldenGoalMatch() || suddenDeathActive)
        {
            yield return StartCoroutine(EndMatchAfterGoal());
            yield break;
        }
        yield return new WaitForSeconds(goalPauseDuration);

        if (matchEnded)
        {
            yield break;
        }

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

        goalText.text = "Goallllllllll";
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

        RestoreBallSkillState();

        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Dynamic;
            ballRb.constraints = RigidbodyConstraints2D.None;
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
    }

    private void FreezeAllBodies()
    {
        FreezeBody(player1);
        FreezeBody(player2);

        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
            ballRb.bodyType = RigidbodyType2D.Kinematic;
            ballRb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void FreezeBody(Transform target)
    {
        if (target == null)
        {
            return;
        }

        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void ResetPositions()
    {
        ResolveRuntimeReferences();

        ResetBody(player1, player1Spawn);
        ResetBody(player2, player2Spawn);

        if (ballRb != null)
        {
            SoictBallStraightShotRuntime straightShotRuntime =
                ballRb.GetComponent<SoictBallStraightShotRuntime>();

            if (straightShotRuntime != null)
            {
                straightShotRuntime.ForceRestoreNormalBall();
            }
        }

        if (ball != null && ballSpawn != null)
        {
            ball.position = ballSpawn.position;
        }

        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Dynamic;
            ballRb.constraints = RigidbodyConstraints2D.None;
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
    }

    private void RestoreBallSkillState()
    {
        if (ball == null)
        {
            return;
        }

        SoictBallStraightShotRuntime straightShotRuntime =
            ball.GetComponent<SoictBallStraightShotRuntime>();

        if (straightShotRuntime != null)
        {
            straightShotRuntime.ForceRestoreNormalBall();
        }
    }

    private void ResetBody(Transform target, Transform spawn)
    {
        if (target == null || spawn == null)
        {
            return;
        }

        target.position = spawn.position;

        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
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

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, matchDuration - timer);
    }

    public float GetMatchDuration()
    {
        return matchDuration;
    }

    public int GetCurrentHalf()
    {
        return 1;
    }

    public int GetPlayer1Score()
    {
        return player1Score;
    }

    public int GetPlayer2Score()
    {
        return player2Score;
    }

    public bool CanUsePlayerActions()
    {
        return matchRunning && !matchEnded && !goalSequenceRunning;
    }

    public bool IsMatchRunning()
    {
        return matchRunning && !matchEnded;
    }

    public bool IsMatchEnded()
    {
        return matchEnded;
    }

    private void SaveMatchResult()
    {
        GameSessionManager session = GameSessionManager.Instance;

        if (session == null)
        {
            GameObject sessionObject = new GameObject("GameSessionManager");
            session = sessionObject.AddComponent<GameSessionManager>();
        }

        string player1CharacterName = GetCharacterDisplayName(player1);
        string player2CharacterName = GetCharacterDisplayName(player2);

        string currentSceneName = SceneManager.GetActiveScene().name;

        session.SetLastMatchResult(
            currentSceneName,
            player1CharacterName,
            player2CharacterName,
            player1Score,
            player2Score
        );
    }

    private string GetCharacterDisplayName(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            return "Player";
        }

        PlayerController2D playerController =
            playerTransform.GetComponent<PlayerController2D>();

        if (playerController == null || playerController.characterData == null)
        {
            return playerTransform.name;
        }

        string characterName = playerController.characterData.name;

        characterName = characterName.Replace("_Data", "");
        characterName = characterName.Replace("Data", "");

        return characterName;
    }

    private IEnumerator LoadResultSceneAfterDelay()
    {
        if (resultSceneDelay > 0f)
        {
            yield return new WaitForSeconds(resultSceneDelay);
        }

        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(matchResultSceneName))
        {
            Debug.LogError("Match result scene name is empty.");
            yield break;
        }

        SceneManager.LoadScene(matchResultSceneName);
    }

    private bool IsGoldenGoalMatch()
    {
        return GameSessionManager.Instance != null &&
            GameSessionManager.Instance.currentGameMode == GameMode.GoldenGoal;
    }

    private IEnumerator EndMatchAfterGoal()
    {
        matchRunning = false;
        matchEnded = true;
        goalSequenceRunning = false;

        FreezeAllBodies();
        UnlockGoalTriggers();

        SaveMatchResult();

        Debug.Log(
            $"Golden Goal ended. Final Score: Player 1 {player1Score} - {player2Score} Player 2"
        );

        if (resultSceneDelay > 0f)
        {
            yield return new WaitForSeconds(resultSceneDelay);
        }

        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(matchResultSceneName))
        {
            Debug.LogError("Match result scene name is empty.");
            yield break;
        }

        SceneManager.LoadScene(matchResultSceneName);
    }

    private bool IsTournamentMatch()
    {
        return GameSessionManager.Instance != null &&
            GameSessionManager.Instance.currentGameMode == GameMode.Tournament;
    }

    public bool IsSuddenDeathActive()
    {
        return suddenDeathActive;
    }

    private void StartSuddenDeath()
    {
        if (suddenDeathActive || matchEnded)
        {
            return;
        }

        suddenDeathActive = true;
        goalSequenceRunning = false;

        HideGoalText();

        StopAllBodies();
        ResetPositions();
        UnlockGoalTriggers();

        Debug.Log("Tournament match is tied. Sudden Death started.");
    }
}