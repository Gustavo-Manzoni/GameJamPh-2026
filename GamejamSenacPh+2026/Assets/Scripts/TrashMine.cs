using System;
using UnityEngine;

public class TrashMine : MonoBehaviour
{
   [SerializeField] GameObject explosionParticle;
    [SerializeField]int pollutionOnExplosion = 10;
    void Start()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            ServiceLocator.Get<GameManager>().IncreasePollutionInstantly(pollutionOnExplosion);
            ServiceLocator.Get<FeedbackManager>().ShakeCamera(.3f, .2f);
            Destroy(gameObject);
        }
    }
}
