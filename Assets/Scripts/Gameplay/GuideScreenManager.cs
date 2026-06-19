using UnityEngine;
using UnityEngine.SceneManagement;

public class GuideScreenManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName = "MainMenuScene";

    public void StartGame()
    {
        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError("Next scene name is empty.");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}