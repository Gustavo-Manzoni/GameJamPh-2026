using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFX
{
    ButtonHover, ButtonClick, ButtonUnhover,
    CardHover, CardUnhover
}

[System.Serializable]
public class SoundConfig
{
    public SFX sound;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    public int poolSize = 3;
    public AudioMixerGroup mixerGroup;
    public bool randomPitch = false;

    [Range(0f, 0.5f)]
    public float pitchVariation = 0.1f;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private List<SoundConfig> soundConfigs;

    private Dictionary<SFX, SoundConfig> _configs = new();
    private Dictionary<SFX, Queue<AudioSource>> _pool = new();



    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitPool();
    }


    private void InitPool()
    {
        foreach (var config in soundConfigs)
        {
            _configs[config.sound] = config;
            _pool[config.sound] = new Queue<AudioSource>();

            for (int i = 0; i < config.poolSize; i++)
                _pool[config.sound].Enqueue(CreateSource(config));
        }
    }

    private AudioSource CreateSource(SoundConfig config)
    {
        var go = new GameObject($"SFX_{config.sound}");
        go.transform.SetParent(transform);

        var src = go.AddComponent<AudioSource>();
        src.clip = config.clip;
        src.volume = config.volume;
        src.outputAudioMixerGroup = config.mixerGroup;
        src.playOnAwake = false;

        return src;
    }

    public void Play(SFX sound)
    {
        if (!_pool.TryGetValue(sound, out var queue) || queue.Count == 0)
        {
            return;
        }

        var config = _configs[sound];
        var source = queue.Dequeue();

        source.pitch = config.randomPitch
            ? 1f + Random.Range(-config.pitchVariation, config.pitchVariation)
            : 1f;

        source.Play();
        StartCoroutine(ReturnToPool(sound, source));
    }


    public void PlayAt(SFX sound, Vector3 position)
    {
        if (!_pool.TryGetValue(sound, out var queue) || queue.Count == 0)
        { 
           
            return;
        }

        var config = _configs[sound];
        var source = queue.Dequeue();

        source.transform.position = position;
        source.spatialBlend = 1f;

        source.pitch = config.randomPitch
            ? 1f + Random.Range(-config.pitchVariation, config.pitchVariation)
            : 1f;

        source.Play();
        StartCoroutine(ReturnToPool(sound, source));
    }


    public void Stop(SFX sound)
    {
        if (!_pool.TryGetValue(sound, out var queue)) return;

        foreach (var source in queue)
            source.Stop();
    }



    private IEnumerator ReturnToPool(SFX sound, AudioSource source)
    {

        yield return new WaitForSeconds(source.clip.length / source.pitch);

        source.Stop();
        source.spatialBlend = 0f;
        source.transform.position = transform.position;

        _pool[sound].Enqueue(source);
    }
}