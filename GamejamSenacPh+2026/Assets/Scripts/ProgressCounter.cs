using System.Collections;
using TMPro;
using UnityEngine;

public class ProgressCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text peopleCountText;
    [SerializeField] private TMP_Text chimneyCountText;

    [SerializeField] private Color increaseFlashColor = Color.green;
    [SerializeField] private Color decreaseFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.25f;

    private int convertedPeopleCount;
    private int totalPeopleCount;
    private int destroyedChimneyCount;
    private int totalChimneyCount;

    private Color peopleBaseColor;
    private Color chimneyBaseColor;
    private Coroutine peopleFlashRoutine;
    private Coroutine chimneyFlashRoutine;

    private void Awake()
    {
        ServiceLocator.Register(this);

        if (peopleCountText != null) peopleBaseColor = peopleCountText.color;
        if (chimneyCountText != null) chimneyBaseColor = chimneyCountText.color;
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
        FlashText(peopleCountText, ref peopleFlashRoutine, increaseFlashColor, peopleBaseColor);
    }

    private void HandlePersonUnconverted()
    {
        if (convertedPeopleCount <= 0) return;
        convertedPeopleCount--;
        UpdatePeopleText();
        FlashText(peopleCountText, ref peopleFlashRoutine, decreaseFlashColor, peopleBaseColor);
    }

    private void HandleChimneyDestroyed()
    {
        destroyedChimneyCount = Mathf.Min(destroyedChimneyCount + 1, totalChimneyCount);
        UpdateChimneyText();
        FlashText(chimneyCountText, ref chimneyFlashRoutine, increaseFlashColor, chimneyBaseColor);
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

    private void FlashText(TMP_Text text, ref Coroutine routine, Color flashColor, Color baseColor)
    {
        if (text == null) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FlashRoutine(text, flashColor, baseColor));
    }

    private IEnumerator FlashRoutine(TMP_Text text, Color flashColor, Color baseColor)
    {
        text.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        text.color = baseColor;
    }
}
