using System;
using UnityEngine;

public class TrashMine : MonoBehaviour
{
   [SerializeField] GameObject explosionParticle;
    int pollution = 10;
    void Start()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
