using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    [SerializeField] GameObject onCollectPersonFeedbackPrefab;
    void Awake()
    {
        ServiceLocator.Register(this);
    }
    public void FeedbackOnCollectPerson(Vector3 position)
    {
        GameObject feedback = Instantiate(onCollectPersonFeedbackPrefab, position, Quaternion.identity);
        Destroy(feedback, 1f);
    }
}
