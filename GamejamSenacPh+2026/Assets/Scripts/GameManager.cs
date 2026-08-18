using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] DemoLoadScene loadScene;
    [SerializeField] float starterDelay;
    void Awake()
    {
        ServiceLocator.Register(this);
    }
    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(starterDelay);
        loadScene.LoadScene(sceneName != string.Empty ? sceneName : SceneManager.GetActiveScene().name);
    }
    public void Win()
    {
        StartCoroutine(WinCoroutine());
        
    }
}
