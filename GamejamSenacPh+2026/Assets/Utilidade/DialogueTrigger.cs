using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(2, 5)]
    [SerializeField] private string[] lines;

    [SerializeField] private bool triggerOnCollision = false;
    [SerializeField] private string requiredTag = "Player";

    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueFinish;

    [ContextMenu("Start Dialogue")]
    public void StartDialogue()
    {
        bool started = DialogueSystem.Instance.StartDialogue(
            lines,
            () => onDialogueStart?.Invoke(),
            () => onDialogueFinish?.Invoke()
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnCollision) return;
        if (!other.CompareTag(requiredTag)) return;
        StartDialogue();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnCollision) return;
        if (!other.CompareTag(requiredTag)) return;
        StartDialogue();
    }
}
