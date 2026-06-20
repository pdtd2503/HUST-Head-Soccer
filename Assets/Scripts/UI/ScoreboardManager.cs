using TMPro;
using UnityEngine;

public class ScoreboardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MatchManager matchManager;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Display Settings")]
    [Tooltip("Số phút hiển thị trên đồng hồ bóng đá. Trận thật vẫn kéo dài theo MatchManager.")]
    [SerializeField] private float displayMatchMinutes = 90f;

    private PlayerController2D player1Controller;
    private PlayerController2D player2Controller;

    private void Start()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (matchManager == null)
        {
            ResolveReferences();

            if (matchManager == null)
            {
                return;
            }
        }

        ResolvePlayerControllers();

        UpdateScoreText();
        UpdateTimeText();
    }

    private void ResolveReferences()
    {
        if (matchManager == null)
        {
            matchManager = FindFirstObjectByType<MatchManager>();
        }

        ResolvePlayerControllers();
    }

    private void ResolvePlayerControllers()
    {
        if (matchManager != null)
        {
            if (player1Controller == null && matchManager.player1 != null)
            {
                player1Controller =
                    matchManager.player1.GetComponent<PlayerController2D>();
            }

            if (player2Controller == null && matchManager.player2 != null)
            {
                player2Controller =
                    matchManager.player2.GetComponent<PlayerController2D>();
            }
        }

        if (player1Controller == null || player2Controller == null)
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

                if (player.isPlayer1)
                {
                    player1Controller = player;
                }
                else
                {
                    player2Controller = player;
                }
            }
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText == null)
        {
            return;
        }

        string player1Name = GetCharacterDisplayName(
            player1Controller,
            "Player 1"
        );

        string player2Name = GetCharacterDisplayName(
            player2Controller,
            "Player 2"
        );

        int player1Score = matchManager.GetPlayer1Score();
        int player2Score = matchManager.GetPlayer2Score();

        scoreText.text =
            $"{player1Name} {player1Score} - {player2Score} {player2Name}";
    }

    private string GetCharacterDisplayName(
        PlayerController2D playerController,
        string fallbackName
    )
    {
        if (playerController == null)
        {
            return fallbackName;
        }

        if (playerController.characterData == null)
        {
            return fallbackName;
        }

        string characterName = playerController.characterData.name;

        characterName = characterName.Replace("_Data", "");
        characterName = characterName.Replace("Data", "");

        return characterName;
    }

    private void UpdateTimeText()
    {
        if (timeText == null)
        {
            return;
        }

        if (matchManager != null && matchManager.IsSuddenDeathActive())
        {
            timeText.text = "SUDDEN DEATH";
            return;
        }

        if (GameSessionManager.Instance != null &&
            GameSessionManager.Instance.currentGameMode == GameMode.GoldenGoal)
        {
            timeText.text = "GOLDEN GOAL";
            return;
        }

        float matchDuration = matchManager.GetMatchDuration();

        if (matchDuration <= 0f)
        {
            matchDuration = 90f;
        }

        float elapsedRealTime = Mathf.Clamp(
            matchManager.GetTimer(),
            0f,
            matchDuration
        );

        float progress = elapsedRealTime / matchDuration;

        int maxDisplaySeconds =
            Mathf.RoundToInt(displayMatchMinutes * 60f);

        int totalDisplaySeconds =
            Mathf.FloorToInt(progress * maxDisplaySeconds);

        if (matchManager.IsMatchEnded())
        {
            totalDisplaySeconds = maxDisplaySeconds;
        }

        totalDisplaySeconds = Mathf.Clamp(
            totalDisplaySeconds,
            0,
            maxDisplaySeconds
        );

        int minutes = totalDisplaySeconds / 60;
        int seconds = totalDisplaySeconds % 60;

        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}