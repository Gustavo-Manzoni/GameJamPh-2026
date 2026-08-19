using UnityEngine;
using DG.Tweening;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

   
    [Range(0f, 1f)] public float musicVolume = 1f;
    [SerializeField] float fadeDuration = 1f;

    [SerializeField]AudioSource musicSource1;
    [SerializeField] AudioSource musicSource2;
    bool isUsingSource1 = true;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        musicSource1.volume = musicVolume;
        musicSource2.volume = musicVolume;
        musicSource1.loop = true;
        musicSource2.loop = true;
        
    }

    public void PlayMusic(AudioClip clip)
    {
      

       if(isUsingSource1)
        {
            isUsingSource1 = false;
            musicSource1.DOFade(0f, fadeDuration-.2f).OnComplete(() =>
            {
              
                musicSource1.Stop();
              
            });
                musicSource2.clip = clip;
                musicSource2.Play();
            musicSource2.DOFade(musicVolume, fadeDuration).OnComplete(() =>
            {
               
            });
        }
        else
        {
            isUsingSource1 = true;
            musicSource1.DOFade(0f, fadeDuration-.2f).OnComplete(() =>
            {
              
                musicSource2.Stop();
              
            });
                musicSource1.clip = clip;
                musicSource1.Play();
            musicSource1.DOFade(musicVolume, fadeDuration).OnComplete(() =>
            {
               
            });
        }
       
    }

    public void SetVolume(float value)
    {
        musicVolume = value;
        musicSource1.volume = value;
        musicSource2.volume = value;
    }
}
