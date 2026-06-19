using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchResultManager : MonoBehaviour
{
    [Header("UI Text")]
    public TMP_Text titleText;
    public TMP_Text scoreText;
    public TMP_Text winnerText;
    public TMP_Text tournamentInfoText;

    [Header("Buttons")]
    public GameObject rematchButton;
    public GameObject nextButton;
    public GameObject mainMenuButton;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenuScene";

    private void Start()
    {
        Time.timeScale = 1f;
        UpdateResultUI();
    }

    private void UpdateResultUI()
    {
        GameSessionManager session = GameSessionManager.Instance;

        if (session == null || !session.hasMatchResult)
        {
            ShowFallbackPreview();
            return;
        }

        if (session.currentGameMode == GameMode.Tournament)
        {
            UpdateTournamentResult(session);
        }
        else
        {
            UpdateNormalResult(session);
        }
    }

    private void UpdateNormalResult(GameSessionManager session)
    {
        if (titleText != null)
        {
            titleText.text = "MATCH RESULT";
        }

        if (scoreText != null)
        {
            scoreText.text =
                $"{session.player1CharacterName} {session.player1Score} - {session.player2Score} {session.player2CharacterName}";
        }

        if (winnerText != null)
        {
            if (session.lastMatchWinner == 0)
            {
                winnerText.text = "Draw";
            }
            else
            {
                winnerText.text = $"Winner: {session.GetLastMatchWinnerName()}";
            }
        }

        if (tournamentInfoText != null)
        {
            tournamentInfoText.gameObject.SetActive(false);
        }

        SetButtonActive(rematchButton, true);
        SetButtonActive(nextButton, false);
        SetButtonActive(mainMenuButton, true);
    }

    private void UpdateTournamentResult(GameSessionManager session)
    {
        bool tournamentComplete = session.IsTournamentComplete();

        if (titleText != null)
        {
            titleText.text = tournamentComplete
                ? "TOURNAMENT CHAMPION"
                : "TOURNAMENT RESULT";
        }

        if (scoreText != null)
        {
            scoreText.text =
                $"{session.player1CharacterName} {session.player1Score} - {session.player2Score} {session.player2CharacterName}";
        }

        if (winnerText != null)
        {
            if (tournamentComplete)
            {
                winnerText.text =
                    $"{session.GetTournamentWinnerName()} wins the tournament!";
            }
            else
            {
                winnerText.text =
                    $"Match Winner: {session.GetLastMatchWinnerName()}";
            }
        }

        if (tournamentInfoText != null)
        {
            tournamentInfoText.gameObject.SetActive(true);
            tournamentInfoText.text =
                $"Tournament Score: {session.player1CharacterName} {session.player1TournamentWins} - {session.player2TournamentWins} {session.player2CharacterName}";
        }

        SetButtonActive(rematchButton, false);
        SetButtonActive(nextButton, !tournamentComplete);
        SetButtonActive(mainMenuButton, tournamentComplete);
    }

    private void ShowFallbackPreview()
    {
        if (titleText != null)
        {
            titleText.text = "MATCH RESULT";
        }

        if (scoreText != null)
        {
            scoreText.text = "No match data";
        }

        if (winnerText != null)
        {
            winnerText.text = "";
        }

        if (tournamentInfoText != null)
        {
            tournamentInfoText.gameObject.SetActive(false);
        }

        SetButtonActive(rematchButton, false);
        SetButtonActive(nextButton, false);
        SetButtonActive(mainMenuButton, true);
    }

    private void SetButtonActive(GameObject buttonObject, bool active)
    {
        if (buttonObject != null)
        {
            buttonObject.SetActive(active);
        }
    }

    public void Rematch()
    {
        GameSessionManager session = GameSessionManager.Instance;

        if (session == null || string.IsNullOrWhiteSpace(session.lastMatchSceneName))
        {
            Debug.LogError("Cannot rematch because last match scene is missing.");
            return;
        }

        SceneManager.LoadScene(session.lastMatchSceneName);
    }

    public void NextTournamentMatch()
    {
        GameSessionManager session = GameSessionManager.Instance;

        if (session == null)
        {
            Debug.LogError("GameSessionManager is missing.");
            return;
        }

        session.AdvanceTournamentMatch();

        string nextMapScene = session.GetRandomTournamentMap();

        if (string.IsNullOrWhiteSpace(nextMapScene))
        {
            Debug.LogError("Cannot load next tournament match because map scene is empty.");
            return;
        }

        GameData.selectedMap = nextMapScene;

        Debug.Log(
            $"Tournament Match {session.currentTournamentMatchIndex} map: {nextMapScene}"
        );

        SceneManager.LoadScene(nextMapScene);
    }

    public void BackToMainMenu()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.SetMode(GameMode.None);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}