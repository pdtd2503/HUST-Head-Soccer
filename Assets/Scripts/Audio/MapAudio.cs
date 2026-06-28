using UnityEngine;

public class MapAudio : MonoBehaviour
{
    public enum AudioType { Map, Menu }

    [SerializeField] private AudioType audioType = AudioType.Map;
    [SerializeField] private string mapName;

    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume = 0.5f;

    private void Start()
    {
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.SetBGMVolume(bgmVolume);

        if (audioType == AudioType.Menu)
        {
            AudioManager.Instance.PlayMenuBGM();
        }
        else
        {
            AudioManager.Instance.PlayBGM(mapName);
        }
    }

    // Bỏ OnDestroy hoàn toàn — không stop BGM khi chuyển scene
}