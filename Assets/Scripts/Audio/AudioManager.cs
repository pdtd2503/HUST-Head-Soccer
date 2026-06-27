using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Skill SFX")]
    [SerializeField] private AudioClip skillSEEE;
    [SerializeField] private AudioClip skillSME;
    [SerializeField] private AudioClip skillSCLS;
    [SerializeField] private AudioClip skillSOICT;

    [Header("SCLS Skill SFX")]
    [SerializeField] private AudioClip sclsThrow;
    [SerializeField] private AudioClip sclsBreak;
    [SerializeField] private AudioClip sclsPoison;

    [Header("Ball SFX")]
    [SerializeField] private AudioClip ballHitPlayer;
    [SerializeField] private AudioClip ballHitObject;

    [Header("Background Music")]
    [SerializeField] private AudioClip bgmNormalMap;
    [SerializeField] private AudioClip bgmMoonMap;
    [SerializeField] private AudioClip bgmWindMap;

    [Header("Match SFX")]
    [SerializeField] private AudioClip whistleStart;  // còi bắt đầu trận
    [SerializeField] private AudioClip whistleEnd;    // còi kết thúc trận
    [SerializeField] private AudioClip goalCheer;     // tiếng hô ghi bàn

    public void PlayWhistleStart()
    {
        PlaySound(whistleStart);
    }

    public void PlayWhistleEnd()
    {
        PlaySound(whistleEnd);
    }

    public void PlayGoalCheer()
    {
        PlaySound(goalCheer);
    }

    private AudioSource audioSource;
    private AudioSource realtimeAudioSource;
    private AudioSource bgmAudioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        realtimeAudioSource = gameObject.AddComponent<AudioSource>();
        realtimeAudioSource.ignoreListenerPause = true;
        realtimeAudioSource.playOnAwake = false;

        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.loop = true;
        bgmAudioSource.playOnAwake = false;
        bgmAudioSource.volume = 0.5f;
    }

    public void PlaySkillSEEE()
    {
        PlaySound(skillSEEE);
    }

    public void PlaySkillSME()
    {
        PlaySound(skillSME);
    }

    public void PlaySkillSCLS()
    {
        PlaySound(skillSCLS);
    }

    public void PlaySkillSOICT()
    {
        PlaySound(skillSOICT);
    }

    public void PlaySkillSCLSThrow()
    {
        PlaySound(sclsThrow);
    }

    public void PlaySkillSCLSBreak()
    {
        PlaySound(sclsBreak);
    }

    public void PlaySkillSCLSPoison()
    {
        PlaySound(sclsPoison);
    }

    public void PlayBallHitPlayer(float volume = 1f)
    {
        PlaySoundWithVolume(ballHitPlayer, volume);
    }

    public void PlayBallHitObject(float volume = 1f)
    {
        PlaySoundWithVolume(ballHitObject, volume);
    }

    public void PlayBGM(string mapName)
    {
        AudioClip clip = null;

        switch (mapName)
        {
            case "NormalMap":
                clip = bgmNormalMap;
                break;
            case "MoonMap":
                clip = bgmMoonMap;
                break;
            case "WindMap":
                clip = bgmWindMap;
                break;
        }

        if (bgmAudioSource == null)
        {
            return;
        }
        
        if (clip == null)
        {
            Debug.LogWarning($"AudioManager: BGM cho map '{mapName}' chưa được gắn!");
            return;
        }

        if (bgmAudioSource.clip == clip) return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        if (bgmAudioSource == null)
        {
            return;
        }

        bgmAudioSource.Stop();
        bgmAudioSource.clip = null;
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmAudioSource == null)
        {
            return;
        }

        bgmAudioSource.volume = Mathf.Clamp01(volume);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: AudioClip is null!");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    private void PlaySoundWithVolume(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: AudioClip is null!");
            return;
        }

        realtimeAudioSource.PlayOneShot(clip, volume);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}