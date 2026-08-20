using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using EasyTransition;

public enum CutsceneCharacter
{
    None,
    Pai,
    Filho
}

[System.Serializable]
public class CutsceneLine
{
    public CutsceneCharacter character;
    public AudioClip clip;
}

public class CutsceneImagesManager : MonoBehaviour
{
    [SerializeField] CutsceneLine[] cutsceneLines;
    [SerializeField] AudioSource audioSource;
    [SerializeField] TMP_Text pressAnyKeyText;
    [SerializeField] CutsceneCinematicJuice cinematicJuice;
    [SerializeField] UnityEvent onCanStart;
    [SerializeField]DemoLoadScene loadScene;
    [SerializeField] string sceneName;
    int currentIndex;
    bool isFirstTime = true;
    void Start()
    {
        
    }
    public void Next()
    {
      
      if(isFirstTime) {isFirstTime= false;return;}
   
      
        
            if (currentIndex >= cutsceneLines.Length)
            {
               loadScene.LoadScene(sceneName != string.Empty ? sceneName : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                return;
            }
            PlayCurrentLine();
    }

    void PlayCurrentLine()
    {
        CutsceneLine line = cutsceneLines[currentIndex];
        audioSource.clip = line.clip;
        audioSource.Play();

        if (cinematicJuice != null)
            cinematicJuice.SetSpeaker(line.character);
    }
    bool hasPressed;
    void Update()
    {
        print(currentIndex);

       if(hasPressed) return;
        foreach(var key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if(Input.GetKeyDown((KeyCode)key))
            {
                hasPressed = true;
        currentIndex = 0;
        PlayCurrentLine();
                onCanStart?.Invoke();
                pressAnyKeyText.DOFade(0, .3f).OnComplete(() => Next());
                break;
            }
        }
    }
}

