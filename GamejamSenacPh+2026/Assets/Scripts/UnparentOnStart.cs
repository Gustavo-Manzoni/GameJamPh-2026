using UnityEngine;

public class UnparentOnStart : MonoBehaviour
{
    void Awake()
    {
        transform.SetParent(null);
    }
}
