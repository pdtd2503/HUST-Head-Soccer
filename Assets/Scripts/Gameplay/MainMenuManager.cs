using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string characterSelectSceneName = "CharacterSelect";

    [Header("Quick Match Characters")]
    public CharacterData[] availableCharacters;
    public bool allowSameCharacterInQuickMatch = false;

    [Header("Quick Match Maps")]
    public string[] quickMatchMapSceneNames =
    {
        "NormalMap",
        "MoonMap",
        "WindMap"
    };

    public void StartQuickMatch()
    {
        EnsureGameSessionManager();

        GameSessionManager.Instance.SetMode(GameMode.QuickMatch);

        if (availableCharacters == null || availableCharacters.Length == 0)
        {
            Debug.LogError("Quick Match has no available characters.");
            return;
        }

        if (quickMatchMapSceneNames == null || quickMatchMapSceneNames.Length == 0)
        {
            Debug.LogError("Quick Match has no map scene names.");
            return;
        }

        CharacterData player1Data = GetRandomCharacter(null);
        CharacterData player2Data = GetRandomCharacter(player1Data);

        GameData.player1Data = player1Data;
        GameData.player2Data = player2Data;

        GameData.player1Character = GetCharacterDisplayName(player1Data);
        GameData.player2Character = GetCharacterDisplayName(player2Data);

        string selectedMapSceneName =
            quickMatchMapSceneNames[Random.Range(0, quickMatchMapSceneNames.Length)];

        GameData.selectedMap = selectedMapSceneName;

        Debug.Log(
            $"Quick Match selected: {GameData.player1Character} vs {GameData.player2Character} on {selectedMapSceneName}"
        );

        SceneManager.LoadScene(selectedMapSceneName);
    }

    public void StartFriendlyMatch()
    {
        EnsureGameSessionManager();

        GameSessionManager.Instance.SetMode(GameMode.FriendlyMatch);

        Debug.Log("Friendly Match selected.");

        SceneManager.LoadScene(characterSelectSceneName);
    }

    public void StartGoldenGoal()
    {
        EnsureGameSessionManager();

        GameSessionManager.Instance.SetMode(GameMode.GoldenGoal);

        Debug.Log("Golden Goal selected.");

        SceneManager.LoadScene(characterSelectSceneName);
    }

    public void StartTournament()
    {
        EnsureGameSessionManager();

        GameSessionManager.Instance.SetMode(GameMode.Tournament);

        Debug.Log("Tournament selected. BO3 mode.");

        SceneManager.LoadScene(characterSelectSceneName);
    }

    private CharacterData GetRandomCharacter(CharacterData excludedCharacter)
    {
        if (availableCharacters.Length == 1 ||
            allowSameCharacterInQuickMatch ||
            excludedCharacter == null)
        {
            return availableCharacters[Random.Range(0, availableCharacters.Length)];
        }

        CharacterData selectedCharacter = null;
        int safetyCounter = 0;

        while (selectedCharacter == null ||
               selectedCharacter == excludedCharacter)
        {
            selectedCharacter =
                availableCharacters[Random.Range(0, availableCharacters.Length)];

            safetyCounter++;

            if (safetyCounter > 20)
            {
                break;
            }
        }

        return selectedCharacter;
    }

    private string GetCharacterDisplayName(CharacterData data)
    {
        if (data == null)
        {
            return "Unknown";
        }

        if (!string.IsNullOrWhiteSpace(data.characterName))
        {
            return data.characterName;
        }

        return data.skillType.ToString();
    }

    private void EnsureGameSessionManager()
    {
        if (GameSessionManager.Instance != null)
        {
            return;
        }

        GameObject sessionObject = new GameObject("GameSessionManager");
        sessionObject.AddComponent<GameSessionManager>();
    }
}