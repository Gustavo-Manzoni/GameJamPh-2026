using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneDialogue : MonoBehaviour
{
 
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private GameObject continuePrompt;

    [TextArea(2, 5)]
    [SerializeField] private string[] lines;

    
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private bool allowSkipTyping = true;

   
    [SerializeField] private bool scaleInOnAppear = true;
    [SerializeField] private float appearDuration = 0.15f;
    [SerializeField] private float startScale = 0f;
    [SerializeField] private AnimationCurve appearCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private float shakeSpeed = 25f;

     [SerializeField] int soundPerLetters;
    public UnityEvent onLineStarted;
    public UnityEvent onLineFinished;
    public UnityEvent onCutsceneFinished;


    private int currentLineIndex = -1;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private List<TextEffect> activeEffects = new List<TextEffect>();
    private float[] revealTime; 

    private TMP_MeshInfo[] cachedMeshInfo;

    private static readonly Regex customTagRegex =
        new Regex(@"<(shake|wave)(?:=([\d.,]+))?>(.*?)</\1>", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex tmpTagRegex = new Regex(@"<[^>]+>", RegexOptions.Compiled);

    private struct TextEffect
    {
        public string type;
        public float intensity;
        public int startIndex;
        public int endIndex;
    }

    private void Awake()
    {
       continuePrompt.SetActive(false);
    }

    private void Start()
    {
        if (lines != null && lines.Length > 0)
            StartCutscene();
    }

    private void Update()
    {
        AnimateText();

        bool clicked = Input.GetMouseButtonDown(0) ||
                       (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (clicked) OnPlayerClick();
    }

  

    public void StartCutscene()
    {
        currentLineIndex = -1;
        NextLine();
    }

    public void OnPlayerClick()
    {
        if (isTyping)
        {
            if (allowSkipTyping) SkipTyping();
        }
        else
        {
            NextLine();
        }
    }

    private void NextLine()
    {
        currentLineIndex++;

        if (lines == null || currentLineIndex >= lines.Length)
        {
            if (continuePrompt != null) continuePrompt.SetActive(false);
            onCutsceneFinished?.Invoke();
            return;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(lines[currentLineIndex]));
    }

    

    private IEnumerator TypeLine(string rawLine)
    {
        isTyping = true;
        if (continuePrompt != null) continuePrompt.SetActive(false);
        onLineStarted?.Invoke();

       
        string cleanText = ParseCustomTags(rawLine, out List<TextEffect> newEffects);

        int cons = 0;
        int[] visibleIndexMap = BuildVisibleIndexMap(cleanText);
        for (int i = 0; i < newEffects.Count; i++)
        {
            TextEffect e = newEffects[i];
            e.startIndex = visibleIndexMap.Length > 0
                ? visibleIndexMap[Mathf.Clamp(e.startIndex, 0, visibleIndexMap.Length - 1)]
                : 0;
            e.endIndex = e.endIndex >= visibleIndexMap.Length
                ? (visibleIndexMap.Length > 0 ? visibleIndexMap[visibleIndexMap.Length - 1] + 1 : 0)
                : visibleIndexMap[e.endIndex];
            newEffects[i] = e;
        }
        activeEffects = newEffects;

        textComponent.text = cleanText;

        
        textComponent.maxVisibleCharacters = int.MaxValue;
        textComponent.ForceMeshUpdate();
        cachedMeshInfo = textComponent.textInfo.CopyMeshInfoVertexData();

        
        textComponent.maxVisibleCharacters = 0;
        textComponent.ForceMeshUpdate();

        int totalCharacters = textComponent.textInfo.characterCount;
        revealTime = new float[totalCharacters];
        for (int i = 0; i < totalCharacters; i++) revealTime[i] = float.MaxValue;

        for (int i = 0; i < totalCharacters; i++)
        {
            bool isWhitespace = textComponent.textInfo.characterInfo[i].character == ' ';

            textComponent.maxVisibleCharacters = i + 1;
            revealTime[i] = Time.time;
            //escreve uma letra
            cons++;
            if (cons == soundPerLetters)
            {
                ServiceLocator.Get<SoundManager>().Play(SFX.SomEscriva);
                cons =0;
            }
            AnimateText();

            if (!isWhitespace)
                yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        if (continuePrompt != null) continuePrompt.SetActive(true);
        onLineFinished?.Invoke();
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        textComponent.maxVisibleCharacters = textComponent.textInfo.characterCount;
        if (revealTime != null)
            for (int i = 0; i < revealTime.Length; i++)
                revealTime[i] = Time.time - appearDuration;

        isTyping = false;
        if (continuePrompt != null) continuePrompt.SetActive(true);
        onLineFinished?.Invoke();
    }


    private string ParseCustomTags(string rawText, out List<TextEffect> effects)
    {
        List<TextEffect> list = new List<TextEffect>();
        int accumulatedOffset = 0;

        string result = customTagRegex.Replace(rawText, match =>
        {
            string type = match.Groups[1].Value.ToLowerInvariant();
            float intensity = 1f;
            if (match.Groups[2].Success)
                float.TryParse(match.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out intensity);
            string content = match.Groups[3].Value;

            int start = match.Index - accumulatedOffset;
            int end = start + content.Length;

            list.Add(new TextEffect { type = type, intensity = intensity, startIndex = start, endIndex = end });

            accumulatedOffset += match.Length - content.Length;
            return content;
        });

        effects = list;
        return result;
    }
    private int[] BuildVisibleIndexMap(string textWithTmpTags)
    {
        int[] map = new int[Mathf.Max(textWithTmpTags.Length, 1)];
        int visible = 0;
        int i = 0;
        while (i < textWithTmpTags.Length)
        {
            if (textWithTmpTags[i] == '<')
            {
                Match m = tmpTagRegex.Match(textWithTmpTags, i);
                if (m.Success && m.Index == i)
                {
                    for (int j = i; j < i + m.Length; j++) map[j] = visible;
                    i += m.Length;
                    continue;
                }
            }
            map[i] = visible;
            visible++;
            i++;
        }
        return map;
    }


    private void AnimateText()
    {
        if (textComponent == null || string.IsNullOrEmpty(textComponent.text) || cachedMeshInfo == null) return;
        if (!scaleInOnAppear && activeEffects.Count == 0) return;

      
        textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

         
            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] destVertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 center = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) * 0.5f;

           
            float scale = 1f;
            if (scaleInOnAppear && revealTime != null && i < revealTime.Length)
            {
                float timeSinceRevealed = Time.time - revealTime[i];
                if (timeSinceRevealed >= 0f && timeSinceRevealed < appearDuration)
                {
                    float t = timeSinceRevealed / appearDuration;
                    scale = Mathf.LerpUnclamped(startScale, 1f, appearCurve.Evaluate(t));
                }
            }

           
            Vector3 offset = Vector3.zero;
            for (int e = 0; e < activeEffects.Count; e++)
            {
                TextEffect effect = activeEffects[e];
                if (i < effect.startIndex || i >= effect.endIndex) continue;

                if (effect.type == "shake") offset += CalculateShake(i, effect.intensity);
                else if (effect.type == "wave") offset += CalculateWave(i, effect.intensity);
            }

            for (int v = 0; v < 4; v++)
            {
                Vector3 original = sourceVertices[vertexIndex + v];
                destVertices[vertexIndex + v] = center + (original - center) * scale + offset;
            }
        }

        for (int m = 0; m < textInfo.meshInfo.Length; m++)
        {
            textInfo.meshInfo[m].mesh.vertices = textInfo.meshInfo[m].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[m].mesh, m);
        }
    }

    private Vector3 CalculateShake(int characterIndex, float intensity)
    {
        float x = (Mathf.PerlinNoise(Time.time * shakeSpeed, characterIndex * 17.13f) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(characterIndex * 17.13f, Time.time * shakeSpeed) - 0.5f) * 2f;
        return new Vector3(x, y, 0f) * intensity;
    }

    private Vector3 CalculateWave(int characterIndex, float intensity)
    {
        float y = Mathf.Sin(Time.time * 5f + characterIndex * 0.5f) * intensity;
        return new Vector3(0f, y, 0f);
    }
}