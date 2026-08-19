using DG.Tweening;
using UnityEngine;

public class HealMine : MonoBehaviour
{
    
   [SerializeField] GameObject explosionParticle;
    [SerializeField]int pollutionHealed = 10;
    [SerializeField] Transform treeVisual, seedVisual;
    [SerializeField] Vector2 particleOffset;
    [SerializeField] float seedFadeTime, treeGrowTime;
    bool grew;
    void Start()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(grew) return;
        if(other.CompareTag("Player"))
        {
            if(explosionParticle != null)
            {
                Instantiate(explosionParticle, (Vector2)transform.position + particleOffset, Quaternion.Euler(0,0,80f));
            }
            grew = true;
            ServiceLocator.Get<GameManager>().IncreasePollutionInstantly(-pollutionHealed);
            ServiceLocator.Get<FeedbackManager>().ShakeCamera(.2f, .2f);
            ServiceLocator.Get<SoundManager>().PlayAt(SFX.Heal, transform.position);
            ServiceLocator.Get<SoundManager>().PlayAt(SFX.Pop, transform.position);
            seedVisual.transform.DOScale(Vector3.zero, seedFadeTime).SetEase(Ease.InBack);
            treeVisual.transform.DOScale(Vector3.one, treeGrowTime).SetEase(Ease.OutBack);
        }
    }
}
