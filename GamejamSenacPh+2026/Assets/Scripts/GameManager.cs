using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEditor.ShaderGraph.Internal;


public class GameManager : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] DemoLoadScene loadScene;
    [SerializeField] float starterDelay;
    [SerializeField] float maxPollution;
    float pollutionIncreaseRatePerSecond;
    [SerializeField] float pollutionAttRate;
    [SerializeField] Image barImage;
    [SerializeField] TMP_Text ratePerSecondText;
    [SerializeField] float normalPersonPollution;
    [SerializeField] float normalFurnacePollution;
    [SerializeField] CanvasGroup losePanel;
    [SerializeField] float losePanelFadeDuration;
    float pollution;
    float timer;
    bool isLoosing;
    public float NormalPersonPollution { get => normalPersonPollution; set => normalPersonPollution = value; }
    public float NormalFurnacePollution { get => normalFurnacePollution; set => normalFurnacePollution = value; }
    public float PollutionIncreaseRatePerSecond { get => pollutionIncreaseRatePerSecond; set => pollutionIncreaseRatePerSecond = value; }
    [SerializeField] SquashStretch barSquashStretch;
      [SerializeField] private Vector2 pollutionUpKick = new Vector2(0.5f, 0.5f);

    void Awake()
    {
        ServiceLocator.Register(this);
        barImage.fillAmount = pollution / maxPollution;
    }
    public void IncreasePollution(float amount)
    {
        PollutionIncreaseRatePerSecond += amount;
        
    }
    public void IncreasePollutionInstantly(int amount)
    {
        pollution += amount;
        barImage.fillAmount = pollution / maxPollution;
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
    public void Lose()
    {
            isLoosing = true;
            losePanel.gameObject.SetActive(true);
            losePanel.DOFade(1, losePanelFadeDuration).OnComplete(() =>
            {
             
            });

    }
    void Update()
    {
        if(isLoosing) return;
        timer += Time.deltaTime;
        if(timer >= pollutionAttRate)
        {
            pollution += PollutionIncreaseRatePerSecond * pollutionAttRate;
            barImage.fillAmount = pollution / maxPollution;
            ratePerSecondText.text = PollutionIncreaseRatePerSecond + "/s";
            timer = 0;
            barSquashStretch.Kick(pollutionUpKick);
            if (pollution >= maxPollution)
            {
                Lose();
            }
        }

    }
}
