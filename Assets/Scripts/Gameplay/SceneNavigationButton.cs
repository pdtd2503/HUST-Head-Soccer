using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigationButton : MonoBehaviour
{
    [Header("Target Scene")]
    public string targetSceneName;

    public void LoadTargetScene()
    {
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogError("Target scene name is empty.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(targetSceneName);
    }
}