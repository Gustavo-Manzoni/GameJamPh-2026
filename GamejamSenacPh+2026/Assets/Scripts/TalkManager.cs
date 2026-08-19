using UnityEngine;

public class TalkManager : MonoBehaviour
{
    public static TalkManager Instance { get; private set; }

    [SerializeField] private TalkPopup talkPopupPrefab;
    [SerializeField] private Vector3 popupOffset = new Vector3(0f, 1.4f, 0f);
    [SerializeField] private Color positiveColor = Color.green;
    [SerializeField] private Color negativeColor = Color.red;

    [TextArea(1, 2)]
    [SerializeField] private string[] positivePhrases;
    [TextArea(1, 2)]
    [SerializeField] private string[] negativePhrases;

    private void Awake()
    {
        Instance = this;
        ServiceLocator.Register(this);
    }

    public void ShowPositive(Vector3 position) => ShowPopup(position, positivePhrases, positiveColor, true);

    public void ShowNegative(Vector3 position) => ShowPopup(position, negativePhrases, negativeColor, false);

    private void ShowPopup(Vector3 position, string[] phrases, Color color, bool isPositive)
    {
        if (talkPopupPrefab == null || phrases == null || phrases.Length == 0) return;

        string phrase = phrases[Random.Range(0, phrases.Length)];
        TalkPopup popup = Instantiate(talkPopupPrefab, position + popupOffset, Quaternion.identity);
        popup.transform.SetParent(transform);
        popup.Show(phrase, color, isPositive);
    }
}
