using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
public class FeedbackManager : MonoBehaviour
{
    [SerializeField] GameObject onCollectPersonFeedbackPrefab;
    PlayerDetectionArea playerDetectionArea;
   
    Camera mainCamera;

    public static FeedbackManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        ServiceLocator.Register(this);
        mainCamera = Camera.main;
    }
    void Start()
    {
        playerDetectionArea = ServiceLocator.Get<PlayerDetectionArea>();
    }
    public void FeedbackOnCollectPerson(Vector3 position)
    {
        if (onCollectPersonFeedbackPrefab == null) return;
        GameObject feedback = Instantiate(onCollectPersonFeedbackPrefab, position, Quaternion.identity);
        Destroy(feedback, 1f);
    }
    public void FeedbackPlayerAreaOnChimneyHit()
    {   
        playerDetectionArea.FeedbackOnChimneyHit();

    }
    

    public void ShakeCamera(float intensity, float duration)
    {
     
      mainCamera.transform.DOShakePosition(duration, intensity, 10, 90f);
    }

   
}
