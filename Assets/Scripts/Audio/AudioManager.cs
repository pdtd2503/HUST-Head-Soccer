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
    [SerializeField] private AudioClip bgmMenu; 

    [Header("Match SFX")]
    [SerializeField] private AudioClip whistleStart;  
    [SerializeField] private AudioClip whistleEnd;    
    [SerializeField] private AudioClip goalCheer;
    [SerializeField] private AudioClip suddenDeathSound; 

    [Header("UI SFX")]
    [SerializeField] private AudioClip buttonClick;
    
    [Header("Obstacle SFX")]
    [SerializeField] private AudioClip obstacleHit;

    public void PlayObstacleHit()
    {
        PlaySound(obstacleHit);
    }
    
    public void PlayButtonClick()
    {
        PlaySound(buttonClick);
    }

    public void PlaySuddenDeath()
    {
        PlaySound(suddenDeathSound);
    }     

    public void PlayWhistleStart()
    {
        PlaySound(whistleStart);
    }

    public void PlayWhistleEnd()
    {
        PlaySound(whistleEnd);
    }

    public void PlayMenuBGM()
    {
        if (bgmAudioSource.clip == bgmMenu) return;
        bgmAudioSource.clip = bgmMenu;
        bgmAudioSource.Play();
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

        public void PlaySkillSEEE(float volume = 1f)
    {
        PlaySoundWithVolume(skillSEEE, volume);
    }

    public void PlaySkillSME(float volume = 1f)
    {
        PlaySoundWithVolume(skillSME, volume);
    }

    public void PlaySkillSCLS(float volume = 1f)
    {
        PlaySoundWithVolume(skillSCLS, volume);
    }

    public void PlaySkillSOICT(float volume = 1f)
    {
        PlaySoundWithVolume(skillSOICT, volume);
    }

    public void PlaySkillSCLSThrow(float volume = 1f)
    {
        PlaySoundWithVolume(sclsThrow, volume);
    }

    public void PlaySkillSCLSBreak(float volume = 1f)
    {
        PlaySoundWithVolume(sclsBreak, volume);
    }

    public void PlaySkillSCLSPoison(float volume = 1f)
    {
        PlaySoundWithVolume(sclsPoison, volume);
    }

    public void PlayBallHitPlayer(float volume = 1.5f)
    {
        PlaySoundWithVolume(ballHitPlayer, volume);
    }

    public void PlayBallHitObject(float volume = 1.5f)
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