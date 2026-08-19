using DG.Tweening;
using TMPro;
using UnityEngine;

public class ProgressCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text peopleCountText;
    [SerializeField] private TMP_Text chimneyCountText;

    [SerializeField] private Color increaseFlashColor = Color.green;
    [SerializeField] private Color decreaseFlashColor = Color.red;
    [SerializeField] private float flashColorDuration = 0.3f;

    [Header("Juice")]
    [SerializeField] private float punchScale = 0.5f;
    [SerializeField] private float punchRotation = 14f;
    [SerializeField] private float punchDuration = 0.5f;
    [SerializeField] private int punchVibrato = 8;
    [SerializeField, Range(0f, 1f)] private float punchElasticity = 0.85f;

    private int convertedPeopleCount;
    private int totalPeopleCount;
    private int destroyedChimneyCount;
    private int totalChimneyCount;

    private Color peopleBaseColor;
    private Color chimneyBaseColor;
    private Vector3 peopleBaseScale;
    private Vector3 chimneyBaseScale;
    private Sequence peopleJuiceSequence;
    private Sequence chimneyJuiceSequence;

    private void Awake()
    {
        ServiceLocator.Register(this);

        if (peopleCountText != null)
        {
            peopleBaseColor = peopleCountText.color;
            peopleBaseScale = peopleCountText.transform.localScale;
        }
        if (chimneyCountText != null)
        {
            chimneyBaseColor = chimneyCountText.color;
            chimneyBaseScale = chimneyCountText.transform.localScale;
        }
    }

    private void Start()
    {
        FollowerPerson[] people = FindObjectsOfType<FollowerPerson>();
        totalPeopleCount = people.Length;
        foreach (var person in people)
        {
            person.OnConverted += HandlePersonConverted;
            person.OnUnconverted += HandlePersonUnconverted;
        }

        Fire[] chimneys = FindObjectsOfType<Fire>();
        totalChimneyCount = chimneys.Length;
        foreach (var chimney in chimneys)
            chimney.OnChimneyDestroyed += HandleChimneyDestroyed;

        UpdatePeopleText();
        UpdateChimneyText();
    }

    private void HandlePersonConverted()
    {
        convertedPeopleCount = Mathf.Min(convertedPeopleCount + 1, totalPeopleCount);
        UpdatePeopleText();
        PlayJuice(peopleCountText, peopleBaseScale, peopleBaseColor, increaseFlashColor, 1f, ref peopleJuiceSequence);
    }

    private void HandlePersonUnconverted()
    {
        if (convertedPeopleCount <= 0) return;
        convertedPeopleCount--;
        UpdatePeopleText();
        PlayJuice(peopleCountText, peopleBaseScale, peopleBaseColor, decreaseFlashColor, -1f, ref peopleJuiceSequence);
    }

    private void HandleChimneyDestroyed()
    {
        destroyedChimneyCount = Mathf.Min(destroyedChimneyCount + 1, totalChimneyCount);
        UpdateChimneyText();
        PlayJuice(chimneyCountText, chimneyBaseScale, chimneyBaseColor, increaseFlashColor, 1f, ref chimneyJuiceSequence);
    }

    private void UpdatePeopleText()
    {
        if (peopleCountText != null)
            peopleCountText.text = $"{convertedPeopleCount}/{totalPeopleCount}";
    }

    private void UpdateChimneyText()
    {
        if (chimneyCountText != null)
            chimneyCountText.text = $"{destroyedChimneyCount}/{totalChimneyCount}";
    }

    // playful punch-scale + wobble + color flash, killing any in-flight juice so rapid changes don't stack
    private void PlayJuice(TMP_Text text, Vector3 baseScale, Color baseColor, Color flashColor, float rotationDirection, ref Sequence sequence)
    {
        if (text == null) return;

        sequence?.Kill();

        Transform t = text.transform;
        t.localScale = baseScale;
        t.localRotation = Quaternion.identity;
        text.color = flashColor;

        Sequence juice = DOTween.Sequence();
        juice.Append(t.DOPunchScale(Vector3.one * punchScale, punchDuration, punchVibrato, punchElasticity));
        juice.Join(t.DOPunchRotation(new Vector3(0f, 0f, punchRotation * rotationDirection), punchDuration, punchVibrato, punchElasticity));
        juice.Join(text.DOColor(baseColor, flashColorDuration).SetDelay(punchDuration * 0.35f));
        juice.OnComplete(() =>
        {
            t.localScale = baseScale;
            t.localRotation = Quaternion.identity;
        });

        sequence = juice;
    }
}
