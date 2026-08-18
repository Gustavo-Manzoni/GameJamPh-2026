
using System;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    SpringVector2 velocitySprintg;
    [SerializeField] float positionVelocity = 10f;  
    [SerializeField] Transform target;
    
    void Update()
    {
        if(target == null) return;
      
    
      
        transform.position = Vector3.Lerp(transform.position, target.position, positionVelocity * Time.deltaTime);  
        
    }
}
