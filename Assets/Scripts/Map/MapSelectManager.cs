using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectManager : MonoBehaviour
{
    public void SelectNormalMap()
    {
        GameData.selectedMap = "Normal";
        SceneManager.LoadScene("NormalMap");
    }

    public void SelectMoonMap()
    {
        GameData.selectedMap = "Moon";
        SceneManager.LoadScene("MoonMap");
    }

    public void SelectWindMap()
    {
        GameData.selectedMap = "Wind";
        SceneManager.LoadScene("WindMap");
    }
}