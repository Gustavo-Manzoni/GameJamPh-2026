using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
public class FeedbackManager : MonoBehaviour
{
    [SerializeField] GameObject onCollectPersonFeedbackPrefab;
    
   
    Camera mainCamera;

    public static FeedbackManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        ServiceLocator.Register(this);
        mainCamera = Camera.main;
    }

    public void FeedbackOnCollectPerson(Vector3 position)
    {
        if (onCollectPersonFeedbackPrefab == null) return;
        GameObject feedback = Instantiate(onCollectPersonFeedbackPrefab, position, Quaternion.identity);
        Destroy(feedback, 1f);
    }

    

    public void ShakeCamera(float intensity, float duration)
    {
     
      mainCamera.transform.DOShakePosition(duration, intensity, 10, 90f);
    }

   
}
