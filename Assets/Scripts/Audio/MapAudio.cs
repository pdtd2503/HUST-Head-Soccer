using UnityEngine;

public class MapAudio : MonoBehaviour
{
    [SerializeField] private string mapName;
    
    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume = 0.5f; // chỉnh volume trực tiếp trong Inspector

    private void Start()
    {
        if (AudioManager.Instance == null) return;
        
        AudioManager.Instance.SetBGMVolume(bgmVolume); // set volume trước
        AudioManager.Instance.PlayBGM(mapName);         // rồi mới play
    }

    private void OnDestroy()
    {
        AudioManager.Instance?.StopBGM();
    }
}