using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeliveryArea : MonoBehaviour
{
    [SerializeField] private Collider2D areaCollider;
    [SerializeField] private float deliverStagger = 0.1f;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
        areaCollider = col;
    }

    private void Awake()
    {
        if (areaCollider == null)
            areaCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
   
      

        FollowChainManager manager = ServiceLocator.Get<FollowChainManager>();
        if (manager == null) return;

        List<FollowerPerson> followers = manager.ClearAllFollowers();
        if (followers.Count == 0) return;

        StartCoroutine(DeliverSequence(followers));
    }

    private IEnumerator DeliverSequence(List<FollowerPerson> followers)
    {
        for (int i = 0; i < followers.Count; i++)
        {
            Vector2 point = GetRandomPointInArea();
            followers[i].transform.position = point;
            followers[i].Deliver();

            if (deliverStagger > 0f)
                yield return new WaitForSeconds(deliverStagger);
        }
    }

    private Vector2 GetRandomPointInArea()
    {
        Bounds bounds = areaCollider.bounds;

        for (int i = 0; i < 30; i++)
        {
            Vector2 point = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y));

            if (areaCollider.OverlapPoint(point))
                return point;
        }

        return bounds.center;
    }
}
