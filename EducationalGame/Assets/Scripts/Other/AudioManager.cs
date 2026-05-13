using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("ћузыка")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip resultMusic;

    [Header("«вуковые эффекты")]
    public AudioClip catchFishSound;      // поймал рыбу
    public AudioClip correctAnswerSound;  // правильный ответ
    public AudioClip wrongAnswerSound;    // неправильный ответ
    public AudioClip splashSound;         // заброс удочки
    public AudioClip levelUpSound;        // повышение уровн€
    public AudioClip gameOverSound;       // конец игры

    private AudioSource musicSource;
    private AudioSource sfxSource;

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

        // ƒва отдельных AudioSource Ч один дл€ музыки, один дл€ эффектов
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
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    public void SetMusicVolume(float volume) => musicSource.volume = volume;

    // === «¬” ќ¬џ≈ Ё‘‘≈ “џ ===
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SetSFXVolume(float volume) => sfxSource.volume = volume;

    // === ”ƒќЅЌџ≈ ћ≈“ќƒџ ===
    public void PlayCatchFish() => PlaySFX(catchFishSound);
    public void PlayCorrectAnswer() => PlaySFX(correctAnswerSound);
    public void PlayWrongAnswer() => PlaySFX(wrongAnswerSound);
    public void PlaySplash() => PlaySFX(splashSound);
    public void PlayLevelUp() => PlaySFX(levelUpSound);
    public void PlayGameOver() => PlaySFX(gameOverSound);
}