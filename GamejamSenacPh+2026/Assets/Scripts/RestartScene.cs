using EasyTransition;
using UnityEngine;

public class RestartScene : MonoBehaviour
{
    [SerializeField] DemoLoadScene loadScene;
    public void Restart()
    {
        loadScene.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
