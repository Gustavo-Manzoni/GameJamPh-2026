using System.Collections.Generic;
using UnityEngine;


public class PositionRecorder : MonoBehaviour
{
    
    [SerializeField] private float pointSpacing = 0.08f;

       [SerializeField] private int maxPoints = 800;

    private readonly List<Vector2> history = new List<Vector2>();

    private void Awake()
    {
        history.Add(transform.position);
    }

    private void LateUpdate()
    {
        Vector2 pos = transform.position;
        Vector2 last = history[history.Count - 1];

        if ((pos - last).sqrMagnitude >= pointSpacing * pointSpacing)
        {
            history.Add(pos);

            if (history.Count > maxPoints)
                history.RemoveRange(0, history.Count - maxPoints);
        }
    }

   
    public Vector2 GetPointAtDistance(float distanceBack)
    {
        if (history.Count == 0) return transform.position;

        float remaining = distanceBack;

        for (int i = history.Count - 1; i > 0; i--)
        {
            float segment = Vector2.Distance(history[i], history[i - 1]);

            if (remaining <= segment)
            {
                float t = segment > 0f ? remaining / segment : 0f;
                return Vector2.Lerp(history[i], history[i - 1], t);
            }

            remaining -= segment;
        }

        return history[0];
    }
}
