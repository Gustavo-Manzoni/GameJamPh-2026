using UnityEngine;

public class ActiveRandom : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToActivate;
    void Awake()
    {
        int randomIndex = Random.Range(0, objectsToActivate.Length);
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].SetActive(i == randomIndex);
        }
    }
}
