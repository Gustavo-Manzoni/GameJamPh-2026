using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void OnInteract() { }
    public virtual void OnLookAt() { }
    public virtual void OnLookAway() { }
}