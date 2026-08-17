using UnityEngine;
using DG.Tweening;
public class WinOnCollide : MonoBehaviour
{
    [SerializeField] float xAmountForEachFollower;
    [SerializeField] float baseMoveTime = 0.5f;
    [SerializeField] float timePerFollower = 0.1f;
    PlayerController _player;
    FollowChainManager _followChainManager;
    void Start()
    {
        _player = ServiceLocator.Get<PlayerController>();
        _followChainManager = ServiceLocator.Get<FollowChainManager>();
        
        }

     private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {   
            if(_followChainManager.ChainCount < _followChainManager.MaxChainCount)
            {
                return;
            }
            _player.CanMove = false;
            _player.transform.DOMove(new Vector3(transform.position.x + xAmountForEachFollower * _followChainManager.ChainCount, transform.position.y, transform.position.z), baseMoveTime + timePerFollower * _followChainManager.ChainCount ).SetEase(Ease.InOutSine).OnComplete(() => {
               
            });


        }
    }
}
