using Unity.Android.Gradle.Manifest;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDetectionArea : MonoBehaviour
{
    [SerializeField] private SquashStretch squashStretch;
    [SerializeField] private Vector2 captureKick = new Vector2(0.5f, 0.5f);

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
    void Awake()
    {
        ServiceLocator.Register(this);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        FollowerPerson person = other.GetComponent<FollowerPerson>();
        if (person == null) return;
        if (!person.IsIdle) return;

        FollowChainManager manager = ServiceLocator.Get<FollowChainManager>();
        if (manager == null) return;

        manager.TryAddFollower(person);

        if (squashStretch != null)
            squashStretch.Kick(captureKick);
    }
    public void FeedbackOnChimneyHit()
    {
          squashStretch.Kick(captureKick);
    }
}