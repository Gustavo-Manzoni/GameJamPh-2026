using UnityEngine;
using DG.Tweening;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [SerializeField] float fadeDuration = 1f;

    AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;

       
        musicSource.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            musicSource.clip = clip;
            musicSource.Play();
            musicSource.DOFade(musicVolume, fadeDuration);
        });
    }

    public void StopMusic()
    {
        musicSource.DOFade(0f, fadeDuration).OnComplete(() => musicSource.Stop());
    }

    public void SetVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = value;
    }
}
