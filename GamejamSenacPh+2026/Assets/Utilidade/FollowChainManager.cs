using System.Collections.Generic;
using UnityEngine;

public class FollowChainManager : MonoBehaviour
{
    public static FollowChainManager Instance { get; private set; }


    [SerializeField] private PositionRecorder playerRecorder;
    [SerializeField] private float spacing = 0.6f;
    [SerializeField] private float shakeOnJoin = 0.12f;

    private readonly List<FollowerPerson> chain = new List<FollowerPerson>();
    private readonly List<Transform> transformsBuffer = new List<Transform>();

    public event System.Action<FollowerPerson, int> OnFollowerJoined;

    int maxChainCount;

    public int ChainCount => chain.Count;

    public int MaxChainCount { get => maxChainCount; set => maxChainCount = value; }


    private void Awake()
    {
        Instance = this;
        ServiceLocator.Register(this);
    }

    public void TryAddFollower(FollowerPerson person)
    {
        if (person == null) return;
        if (!person.IsIdle) return;
        if (chain.Contains(person)) return;

        PositionRecorder targetRecorder = chain.Count == 0
            ? playerRecorder
            : chain[chain.Count - 1].Recorder;

        person.JoinChain(targetRecorder, spacing);
        chain.Add(person);

        if (shakeOnJoin > 0f)
            CameraShake.Instance?.AddTrauma(shakeOnJoin);

        OnFollowerJoined?.Invoke(person, chain.Count - 1);
    }

    public void RemoveFollower(FollowerPerson person)
    {
        int index = chain.IndexOf(person);
        if (index == -1) return;

        chain.RemoveAt(index);

        for (int i = index; i < chain.Count; i++)
        {
            PositionRecorder newTarget = i == 0 ? playerRecorder : chain[i - 1].Recorder;
            chain[i].SetTarget(newTarget, spacing);
        }
    }

      public IReadOnlyList<Transform> GetChainTransforms(Transform player)
    {
        transformsBuffer.Clear();
        if (player != null) transformsBuffer.Add(player);
        for (int i = 0; i < chain.Count; i++)
            transformsBuffer.Add(chain[i].transform);
        return transformsBuffer;
    }
}
