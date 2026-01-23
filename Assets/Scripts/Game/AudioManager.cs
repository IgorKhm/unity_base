using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip musicLoop;
    public AudioClip shoot;
    public AudioClip hit;
    public AudioClip enemyDie;
    public AudioClip playerHit;
    public AudioClip uiClick;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        // If you only have one scene, you can skip DontDestroyOnLoad.
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (musicSource != null && musicLoop != null)
        {
            musicSource.clip = musicLoop;
            musicSource.loop = true;
            if (!musicSource.isPlaying) musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        Debug.Log($"PlaySFX called. clip={(clip?clip.name:"NULL")} sfxSource={(sfxSource? sfxSource.name:"NULL")} vol={volume}");

        if (clip == null || sfxSource == null) return;

        Debug.Log($"Before: isPlaying={sfxSource.isPlaying} spatialBlend={sfxSource.spatialBlend} vol={sfxSource.volume} mute={sfxSource.mute} output={sfxSource.outputAudioMixerGroup}");

        sfxSource.PlayOneShot(clip, volume);

        Debug.Log($"After: isPlaying={sfxSource.isPlaying} time={sfxSource.time}");
    }

    public void PlayShoot()
    {
        Debug.Log("shoooooot");
        PlaySFX(shoot);
    }

    public void PlayHit()     => PlaySFX(hit);
    public void PlayEnemyDie()=> PlaySFX(enemyDie);
    public void PlayPlayerHit()=> PlaySFX(playerHit);
    public void PlayUIClick() => PlaySFX(uiClick);
}