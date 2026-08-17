using UnityEngine;

public class TopDownInteractions : MonoBehaviour
{
    private Interactable currentInteractable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null){
            return;
        }

        if (currentInteractable != null){ currentInteractable.OnLookAway();}

        currentInteractable = interactable;
        currentInteractable.OnLookAt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable == null || interactable != currentInteractable)
            return;

        currentInteractable.OnLookAway();
        currentInteractable = null;
    }

    private void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
            currentInteractable.OnInteract();
    }
}
