using UnityEngine;
using DG.Tweening;
using EasyTransition;

public class WinOnCollide : MonoBehaviour
{
    [SerializeField] float xAmountForEachFollower;
    [SerializeField] float baseMoveTime = 0.5f;
    [SerializeField] float timePerFollower = 0.1f;
    PlayerController _player;
    FollowChainManager _followChainManager;
    GameManager _gameManager;
    [SerializeField] string nextScene;
    void Start()
    {
        _player = ServiceLocator.Get<PlayerController>();
        _followChainManager = ServiceLocator.Get<FollowChainManager>();
        _gameManager = ServiceLocator.Get<GameManager>();
    }

     private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {   
            if(_gameManager.PollutionIncreaseRatePerSecond != 0)
            {
                return;
            }
            _player.CanMove = false;
            _player.transform.DOMove(new Vector3(transform.position.x, transform.position.y, transform.position.z), baseMoveTime + timePerFollower * _followChainManager.ChainCount ).SetEase(Ease.InOutSine).OnComplete(() => {
               
            });
            _gameManager.Win();


        }
    }
}
