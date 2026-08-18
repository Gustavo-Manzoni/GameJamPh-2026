using UnityEngine;
using System.Collections.Generic;
public class DamageFireOverTime : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float timePerDamage;
    List<IFireable> allFires = new List<IFireable>();
    float timer;
    void Start()
    {
        
    }
    void Update()
    {
       
        timer += Time.deltaTime;
        if (allFires.Count == 0) return;
        if(timer >= timePerDamage) 
        {
            foreach(IFireable item in allFires)
            {
                if (item == null) { allFires.Remove(item); continue; }

                item.TakeDamage(damage);
            
            }
            timer = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent(out IFireable fire)) 
        {
            allFires.Add(fire);
        
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IFireable fire))
        {
            allFires.Remove(fire);

        }
    }
}
