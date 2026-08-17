using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class RopeVisual : MonoBehaviour
{
        public float segmentLength = 0.25f;
    [Range(1, 30)]
    public int constraintIterations = 12;
    public float gravity = 9.8f;
    [Range(0f, 1f)]
    public float damping = 0.98f;

    public float flightSlackMultiplier = 1.35f;

    LineRenderer line;
    Vector2[] points;
    Vector2[] oldPoints;
    int segmentCount;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }

       public void Begin(Vector2 start, Vector2 end)
    {
        float dist = Vector2.Distance(start, end);
        segmentCount = Mathf.Max(2, Mathf.CeilToInt(dist / segmentLength) + 1);

        points = new Vector2[segmentCount];
        oldPoints = new Vector2[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            points[i] = Vector2.Lerp(start, end, t);
            oldPoints[i] = points[i];
        }

        line.positionCount = segmentCount;
        line.enabled = true;
    }

    public void Stop()
    {
        line.enabled = false;
        points = null;
    }

    public void UpdateRope(Vector2 start, Vector2 end, bool isTaut)
    {
        if (points == null) return;

        float realDistance = Vector2.Distance(start, end);
        float targetLength = isTaut ? realDistance : realDistance * flightSlackMultiplier;

        int desiredCount = Mathf.Max(2, Mathf.CeilToInt(targetLength / segmentLength) + 1);
        if (desiredCount != segmentCount)
            ResizeSegments(desiredCount, start, end);

        Simulate(start, end);
        Constrain(start, end, targetLength);

        line.positionCount = segmentCount;
        for (int i = 0; i < segmentCount; i++)
            line.SetPosition(i, points[i]);
    }

    void ResizeSegments(int newCount, Vector2 start, Vector2 end)
    {
        Vector2[] newPoints = new Vector2[newCount];
        Vector2[] newOld = new Vector2[newCount];

        for (int i = 0; i < newCount; i++)
        {
            float t = i / (float)(newCount - 1);
            int oldIndex = Mathf.Clamp(Mathf.RoundToInt(t * (segmentCount - 1)), 0, segmentCount - 1);
            newPoints[i] = (points != null && points.Length > 0) ? points[oldIndex] : Vector2.Lerp(start, end, t);
            newOld[i] = newPoints[i];
        }

        points = newPoints;
        oldPoints = newOld;
        segmentCount = newCount;
    }

    void Simulate(Vector2 start, Vector2 end)
    {
        for (int i = 1; i < segmentCount - 1; i++)
        {
            Vector2 velocity = (points[i] - oldPoints[i]) * damping;
            oldPoints[i] = points[i];
            points[i] += velocity;
            points[i] += Vector2.down * gravity * Time.deltaTime * Time.deltaTime;
        }

        points[0] = start;
        points[segmentCount - 1] = end;
    }

    void Constrain(Vector2 start, Vector2 end, float targetLength)
    {
        float segLen = targetLength / (segmentCount - 1);

        for (int iteration = 0; iteration < constraintIterations; iteration++)
        {
            points[0] = start;
            points[segmentCount - 1] = end;

            for (int i = 0; i < segmentCount - 1; i++)
            {
                Vector2 delta = points[i + 1] - points[i];
                float dist = delta.magnitude;
                if (dist < 0.0001f) continue;

                float error = dist - segLen;
                Vector2 correction = delta.normalized * error * 0.5f;

                if (i != 0)
                    points[i] += correction;
                if (i + 1 != segmentCount - 1)
                    points[i + 1] -= correction;
            }
        }
    }
}