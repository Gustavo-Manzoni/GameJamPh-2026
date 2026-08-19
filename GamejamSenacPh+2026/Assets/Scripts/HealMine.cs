using UnityEngine;

public class HealMine : MonoBehaviour
{
    
   [SerializeField] GameObject explosionParticle;
    [SerializeField]int pollutionHealed = 10;
    void Start()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(explosionParticle != null)
            {
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
            }
            
            ServiceLocator.Get<GameManager>().IncreasePollutionInstantly(-pollutionHealed);
            ServiceLocator.Get<FeedbackManager>().ShakeCamera(.2f, .2f);
            Destroy(gameObject);
        }
    }
}
