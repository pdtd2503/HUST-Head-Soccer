using UnityEngine;

public class MapAudio : MonoBehaviour
{
    [SerializeField] private string mapName;

    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume = 0.5f;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.SetBGMVolume(bgmVolume);
        AudioManager.Instance.PlayBGM(mapName);
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.StopBGM();
    }
}