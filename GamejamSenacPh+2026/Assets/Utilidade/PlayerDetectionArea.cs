using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class PlayerDetectionArea : MonoBehaviour
{
    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FollowerPerson person = other.GetComponent<FollowerPerson>();
        if (person == null) return;

        FollowChainManager.Instance?.TryAddFollower(person);
    }
}
