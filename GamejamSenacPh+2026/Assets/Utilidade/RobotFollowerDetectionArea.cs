using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class RobotFollowerDetectionArea : MonoBehaviour
{
    [SerializeField] private RobotPatrol robot;
    [SerializeField] SquashStretch squashStretch;
       [SerializeField] private Vector2 captureKick = new Vector2(0.5f, 0.5f);

    private void Reset()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;

    }

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FollowerPerson follower = other.GetComponent<FollowerPerson>();
        if (follower != null)
        {
            robot?.OnFollowerDetected(follower);
               squashStretch.Kick(captureKick);
        }
        
    }
}
