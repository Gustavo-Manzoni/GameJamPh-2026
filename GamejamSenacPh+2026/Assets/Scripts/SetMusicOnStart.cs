using UnityEngine;

public class SetMusicOnStart : MonoBehaviour
{
    [SerializeField] AudioClip musicClip;
    void Start()
    {
        MusicManager.Instance.PlayMusic(musicClip);
    }
}
