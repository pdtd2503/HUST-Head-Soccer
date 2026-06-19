using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenuScene";

    private bool isPaused;

    private void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

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

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
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