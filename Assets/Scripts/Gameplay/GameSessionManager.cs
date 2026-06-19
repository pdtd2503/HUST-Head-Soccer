using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    None,
    QuickMatch,
    FriendlyMatch,
    GoldenGoal,
    Tournament
}

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [Header("Current Mode")]
    public GameMode currentGameMode = GameMode.None;

    [Header("Last Match Data")]
    public bool hasMatchResult;
    public string lastMatchSceneName;
    public string player1CharacterName = "Player 1";
    public string player2CharacterName = "Player 2";
    public int player1Score;
    public int player2Score;
    public int lastMatchWinner;

    [Header("Tournament Data")]
    public int player1TournamentWins;
    public int player2TournamentWins;
    public int currentTournamentMatchIndex;
    public int targetTournamentWins = 2;

    [Header("Tournament Maps")]
    public string[] tournamentMapSceneNames =
    {
        "NormalMap",
        "MoonMap",
        "WindMap"
    };

    private readonly List<string> remainingTournamentMaps = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMode(GameMode mode)
    {
        currentGameMode = mode;
        ClearLastMatchResult();

        if (mode == GameMode.Tournament)
        {
            ResetTournament();
        }
    }

    public void SetLastMatchResult(
        string matchSceneName,
        string p1CharacterName,
        string p2CharacterName,
        int p1Score,
        int p2Score
    )
    {
        hasMatchResult = true;

        lastMatchSceneName = matchSceneName;
        player1CharacterName = p1CharacterName;
        player2CharacterName = p2CharacterName;
        player1Score = p1Score;
        player2Score = p2Score;

        if (player1Score > player2Score)
        {
            lastMatchWinner = 1;
        }
        else if (player2Score > player1Score)
        {
            lastMatchWinner = 2;
        }
        else
        {
            lastMatchWinner = 0;
        }

        if (currentGameMode == GameMode.Tournament && lastMatchWinner != 0)
        {
            RegisterTournamentWin(lastMatchWinner);
        }
    }

    public void ClearLastMatchResult()
    {
        hasMatchResult = false;
        lastMatchSceneName = "";
        player1Score = 0;
        player2Score = 0;
        lastMatchWinner = 0;
    }

    public void ResetTournament()
    {
        player1TournamentWins = 0;
        player2TournamentWins = 0;
        currentTournamentMatchIndex = 1;
        targetTournamentWins = 2;

        ResetTournamentMaps();
    }

    private void ResetTournamentMaps()
    {
        remainingTournamentMaps.Clear();

        if (tournamentMapSceneNames == null)
        {
            return;
        }

        for (int i = 0; i < tournamentMapSceneNames.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(tournamentMapSceneNames[i]))
            {
                remainingTournamentMaps.Add(tournamentMapSceneNames[i]);
            }
        }
    }

    public string GetRandomTournamentMap()
    {
        if (remainingTournamentMaps.Count == 0)
        {
            ResetTournamentMaps();
        }

        if (remainingTournamentMaps.Count == 0)
        {
            Debug.LogError("No tournament maps available.");
            return "";
        }

        int randomIndex = Random.Range(0, remainingTournamentMaps.Count);
        string selectedMap = remainingTournamentMaps[randomIndex];

        remainingTournamentMaps.RemoveAt(randomIndex);

        return selectedMap;
    }

    public void AdvanceTournamentMatch()
    {
        currentTournamentMatchIndex++;
    }

    private void RegisterTournamentWin(int winner)
    {
        if (winner == 1)
        {
            player1TournamentWins++;
        }
        else if (winner == 2)
        {
            player2TournamentWins++;
        }
    }

    public bool IsTournamentComplete()
    {
        return player1TournamentWins >= targetTournamentWins ||
               player2TournamentWins >= targetTournamentWins;
    }

    public string GetLastMatchWinnerName()
    {
        if (lastMatchWinner == 1)
        {
            return player1CharacterName;
        }

        if (lastMatchWinner == 2)
        {
            return player2CharacterName;
        }

        return "Draw";
    }

    public string GetTournamentWinnerName()
    {
        if (player1TournamentWins >= targetTournamentWins)
        {
            return player1CharacterName;
        }

        if (player2TournamentWins >= targetTournamentWins)
        {
            return player2CharacterName;
        }

        return "";
    }

    public bool IsGoldenGoalMode()
    {
        return currentGameMode == GameMode.GoldenGoal;
    }

    public bool IsTournamentMode()
    {
        return currentGameMode == GameMode.Tournament;
    }

    public bool IsQuickMatchMode()
    {
        return currentGameMode == GameMode.QuickMatch;
    }

    public bool IsFriendlyMatchMode()
    {
        return currentGameMode == GameMode.FriendlyMatch;
    }
}