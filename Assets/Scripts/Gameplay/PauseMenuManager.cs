using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenuScene";

    private bool isPaused;
    private MatchManager matchManager;

    private void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        matchManager = FindFirstObjectByType<MatchManager>();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            return;
        }

        if (matchManager != null && matchManager.IsGoalSequenceRunning())
        {
            return;
        }

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            pausePanel.transform.SetAsLastSibling();
        }
    }

    public void ResumeGame()
    {
        if (!isPaused)
        {
            return;
        }

        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;

        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.SetMode(GameMode.None);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}