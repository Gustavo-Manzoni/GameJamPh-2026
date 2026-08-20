using UnityEngine;
using DG.Tweening;
using GameJam.Utilities;
[System.Serializable]
public class MenuContainers
{
    [SerializeField] RectTransform _container;
    [SerializeField] RectTransform _finalPosition;
    [SerializeField] float delayBetweenContainers = 0.1f;
    [SerializeField] float moveTime = 0.5f;
   
    Vector2 _starterPosition;

    public Vector2 StarterPosition { get => _starterPosition; set => _starterPosition = value; }
    
    public Tween MoveToFinalPosition(Ease ease, float easeOvershoot, float startDelay = 0f)
    {
        return _container.DOAnchorPos(_finalPosition.anchoredPosition, moveTime)
            .SetEase(ease, easeOvershoot)
            .SetDelay(startDelay + delayBetweenContainers);
    }
    
    public Tween MoveToStarterPosition(Ease ease, float easeOvershoot, float startDelay = 0f)
    {
        return _container.DOAnchorPos(_starterPosition, moveTime)
            .SetEase(ease, easeOvershoot)
            .SetDelay(startDelay + delayBetweenContainers);
    }
    
    public void Init()
    {
        StarterPosition = _container.anchoredPosition;
    }
}
public enum MainMenuState
{
    MainMenu,
    Options,
    
}
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] MenuContainers[] normalMenuContainers;
    [SerializeField] MenuContainers[] optionsMenuContainers;
    [SerializeField] Ease moveEase = Ease.OutBack;
    [SerializeField] float easeOvershoot = 1;
    [SerializeField] float delayBetweenStateChange = 0.2f;
    
    MainMenuState currentState = MainMenuState.MainMenu;
    private Sequence _currentSequence;

    CooldownTimer clickOptionsCooldown;
    
    
    void Awake()
    {
        ServiceLocator.Register(this);
        foreach (var container in normalMenuContainers)
        {
            container.Init();
        }
        foreach (var container in optionsMenuContainers)
        {
            container.Init();
        }
        clickOptionsCooldown = new CooldownTimer(0.5f);
    }
    void Update()
    {
        clickOptionsCooldown.Tick();
    }
    public void Leave(){ Application.Quit();}
    public void OnClickOptions()
    {
        if(!clickOptionsCooldown.CanUse)return;
        clickOptionsCooldown.Use();
         _currentSequence?.Kill();
        _currentSequence = DOTween.Sequence();
        
        switch(currentState)
        {
            case MainMenuState.MainMenu:
                
                for (int i = 0; i < normalMenuContainers.Length; i++)
                {
                    _currentSequence.Join(normalMenuContainers[i].MoveToFinalPosition(moveEase, easeOvershoot, i * delayBetweenStateChange));
                }
                
                 for (int i = 0; i < optionsMenuContainers.Length; i++)
                {
                    _currentSequence.Join(optionsMenuContainers[i].MoveToFinalPosition(moveEase, easeOvershoot, i * delayBetweenStateChange));
                }
                
                currentState = MainMenuState.Options;
                break;
                
            case MainMenuState.Options:
           
                for (int i = optionsMenuContainers.Length - 1; i >= 0; i--)
                {
                    _currentSequence.Join(optionsMenuContainers[i].MoveToStarterPosition(moveEase, easeOvershoot, (optionsMenuContainers.Length - 1 - i) * delayBetweenStateChange));
                }
                
               
                for (int i = 0; i < normalMenuContainers.Length; i++)
                {
                    _currentSequence.Join(normalMenuContainers[i].MoveToStarterPosition(moveEase, easeOvershoot, i * delayBetweenStateChange));
                }
                
                currentState = MainMenuState.MainMenu;
                break;
        }
    }
}
