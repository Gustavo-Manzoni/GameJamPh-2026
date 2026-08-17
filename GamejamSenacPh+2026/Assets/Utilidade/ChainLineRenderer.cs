using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ChainLineRenderer : MonoBehaviour
{
    [SerializeField] private FollowChainManager chainManager;
    [SerializeField] private Transform player;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void LateUpdate()
    {
        if (chainManager == null || player == null) return;

        IReadOnlyList<Transform> points = chainManager.GetChainTransforms(player);
        line.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
            line.SetPosition(i, points[i].position);
    }
}
