using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
public class CutsceneImagesManager : MonoBehaviour
{
    [SerializeField]AudioClip[] cutsceneImages;
    [SerializeField] AudioSource audioSource;
    [SerializeField] TMP_Text pressAnyKeyText;
    [SerializeField] UnityEvent onCanStart;
    int currentIndex;
    void Start()
    {
        
        currentIndex = 0;
        audioSource.clip = cutsceneImages[currentIndex];
        audioSource.Play();
    }
    public void Next()
    {
      
          currentIndex++;
            if (currentIndex >= cutsceneImages.Length)
            {
               
                return;
            }
            audioSource.clip = cutsceneImages[currentIndex];
            audioSource.Play();
    }
    bool hasPressed;
    void Update()
    {

       if(hasPressed) return;
        foreach(var key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if(Input.GetKeyDown((KeyCode)key))
            {
                hasPressed = true;
                onCanStart?.Invoke();
                pressAnyKeyText.DOFade(0, .3f).OnComplete(() => Next());
                break;
            }
        }
    }
}
