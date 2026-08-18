using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Net.Mail;
using TMPro;


public class GameManager : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] DemoLoadScene loadScene;
    [SerializeField] float starterDelay;
    [SerializeField] float maxPollution;
    [SerializeField] float pollutionIncreaseRatePerSecond;
    [SerializeField] float pollutionAttRate;
    [SerializeField] Image barImage;
    [SerializeField] TMP_Text ratePerSecondText;
    [SerializeField] float normalPersonPollution;
    [SerializeField] float normalFurnacePollution;
    float pollution;
    float timer;

    public float NormalPersonPollution { get => normalPersonPollution; set => normalPersonPollution = value; }
    public float NormalFurnacePollution { get => normalFurnacePollution; set => normalFurnacePollution = value; }

    void Awake()
    {
        ServiceLocator.Register(this);
    }
    public void IncreasePollution(float amount)
    {
        pollutionIncreaseRatePerSecond += amount;
        
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
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= pollutionAttRate)
        {
            pollution += pollutionIncreaseRatePerSecond * pollutionAttRate;
            barImage.fillAmount = pollution / maxPollution;
            ratePerSecondText.text = pollutionIncreaseRatePerSecond + "/s";
            timer = 0;

        }

    }
}
