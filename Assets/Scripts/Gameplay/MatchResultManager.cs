using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchResultManager : MonoBehaviour
{
    [Header("Result Texts")]
    public TMP_Text titleText;
    public TMP_Text scoreText;
    public TMP_Text winnerText;
    public TMP_Text tournamentInfoText;

    [Header("Character Result UI")]
    public Image player1ResultImage;
    public Image player2ResultImage;
    public TMP_Text player1ResultNameText;
    public TMP_Text player2ResultNameText;

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

        UpdateCharacterImages(session);

        if (session.currentGameMode == GameMode.Tournament)
        {
            UpdateTournamentResult(session);
        }
        else
        {
            UpdateNormalResult(session);
        }
    }

    private void UpdateCharacterImages(GameSessionManager session)
    {
        CharacterData player1Data = GameData.player1Data;
        CharacterData player2Data = GameData.player2Data;

        if (player1ResultImage != null && player1Data != null)
        {
            player1ResultImage.sprite = player1Data.headRightSprite;
            player1ResultImage.preserveAspect = true;
        }

        if (player2ResultImage != null && player2Data != null)
        {
            player2ResultImage.sprite = player2Data.headLeftSprite;
            player2ResultImage.preserveAspect = true;
        }

        if (player1ResultNameText != null)
        {
            player1ResultNameText.text =
                "PLAYER 1\n" + GetCharacterDisplayName(player1Data, session.player1CharacterName);
        }

        if (player2ResultNameText != null)
        {
            player2ResultNameText.text =
                "PLAYER 2\n" + GetCharacterDisplayName(player2Data, session.player2CharacterName);
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
            winnerText.text = BuildWinnerText(session, false);
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
            titleText.text = "MATCH RESULT";
        }

        if (scoreText != null)
        {
            scoreText.text =
                $"{session.player1CharacterName} {session.player1Score} - {session.player2Score} {session.player2CharacterName}";
        }

        if (winnerText != null)
        {
            winnerText.text = BuildWinnerText(session, tournamentComplete);
        }

        if (tournamentInfoText != null)
        {
            tournamentInfoText.gameObject.SetActive(true);
            tournamentInfoText.text =
                $"TOURNAMENT SCORE: PLAYER 1 {session.player1TournamentWins} - {session.player2TournamentWins} PLAYER 2";
        }

        SetButtonActive(rematchButton, false);
        SetButtonActive(nextButton, !tournamentComplete);
        SetButtonActive(mainMenuButton, tournamentComplete);
    }

    private string BuildWinnerText(GameSessionManager session, bool tournamentComplete)
    {
        if (session.lastMatchWinner == 1)
        {
            string label = tournamentComplete ? "TOURNAMENT WINNER" : "WINNER";
            return $"{label}: PLAYER 1\n{session.player1CharacterName}";
        }

        if (session.lastMatchWinner == 2)
        {
            string label = tournamentComplete ? "TOURNAMENT WINNER" : "WINNER";
            return $"{label}: PLAYER 2\n{session.player2CharacterName}";
        }

        return "DRAW";
    }

    private string GetCharacterDisplayName(CharacterData data, string fallbackName)
    {
        if (data != null && !string.IsNullOrWhiteSpace(data.characterName))
        {
            return data.characterName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(fallbackName))
        {
            return fallbackName.Trim();
        }

        return "UNKNOWN";
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

        if (player1ResultNameText != null)
        {
            player1ResultNameText.text = "PLAYER 1";
        }

        if (player2ResultNameText != null)
        {
            player2ResultNameText.text = "PLAYER 2";
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