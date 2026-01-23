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
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayShoot() => PlaySFX(shoot);
    public void PlayHit()     => PlaySFX(hit);
    public void PlayEnemyDie()=> PlaySFX(enemyDie);
    public void PlayPlayerHit()=> PlaySFX(playerHit);
    public void PlayUIClick() => PlaySFX(uiClick);
}