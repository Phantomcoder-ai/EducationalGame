using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("ћузыка")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip resultMusic;

    [Header("«вуковые эффекты")]
    public AudioClip catchFishSound;
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;
    public AudioClip splashSound;
    public AudioClip levelUpSound;
    public AudioClip gameOverSound;
    public AudioClip sharkSound;
    public AudioClip fuguSound;
    public AudioClip goldenFishSound;
    public AudioClip timerUrgentSound;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private bool timerUrgentPlaying = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            musicSource = sources[0];
            sfxSource = sources[1];
        }
        else
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        sfxSource.loop = false;
    }

    // === ћ”«џ ј ===
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();
    public void SetMusicVolume(float v) => musicSource.volume = v;
    public void SetSFXVolume(float v) => sfxSource.volume = v;

    // === Ё‘‘≈ “џ ===
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // === ”ƒќЅЌџ≈ ћ≈“ќƒџ ===
    public void PlayCatch() => PlaySFX(catchFishSound);
    public void PlayCorrect() => PlaySFX(correctAnswerSound);
    public void PlayWrong() => PlaySFX(wrongAnswerSound);
    public void PlaySplash() => PlaySFX(splashSound);
    public void PlayLevelUp() => PlaySFX(levelUpSound);
    public void PlayGameOver() => PlaySFX(gameOverSound);
    public void PlayShark() => PlaySFX(sharkSound);
    public void PlayFugu() => PlaySFX(fuguSound);
    public void PlayGoldenFish() => PlaySFX(goldenFishSound);

    // —рочный таймер Ч зацикленный звук
    public void StartUrgentTimer()
    {
        if (timerUrgentPlaying || timerUrgentSound == null) return;
        sfxSource.clip = timerUrgentSound;
        sfxSource.loop = true;
        sfxSource.Play();
        timerUrgentPlaying = true;
    }

    public void StopUrgentTimer()
    {
        if (!timerUrgentPlaying) return;
        sfxSource.loop = false;
        sfxSource.Stop();
        timerUrgentPlaying = false;
    }
}