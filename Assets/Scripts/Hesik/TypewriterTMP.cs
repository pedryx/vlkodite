using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class TypewriterTMPFading : MonoBehaviour
{
    [Header("Text Settings")]
    [TextArea][SerializeField] private string fullText;
    [SerializeField] private float delayBetweenChars = 0.05f;
    [SerializeField] private float hesitationDelay = 0.5f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private bool playOnStart = true;

    [Tooltip("Use '^' in text to trigger hesitation pauses")]
    [SerializeField] private char hesitationMarker = '^';

    private TMP_Text tmpText;
    private Coroutine typingCoroutine;
    private string cleanText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        cleanText = fullText.Replace(hesitationMarker.ToString(), "");
        tmpText.text = cleanText;
        tmpText.ForceMeshUpdate();
        MakeAllCharactersInvisible();
    }

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        cleanText = fullText.Replace(hesitationMarker.ToString(), "");
        tmpText.text = cleanText;
        tmpText.ForceMeshUpdate();
        MakeAllCharactersInvisible();

        typingCoroutine = StartCoroutine(TypeWithFade());
    }

    private void MakeAllCharactersInvisible()
    {
        TMP_TextInfo textInfo = tmpText.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Color32[] vertexColors = textInfo.meshInfo[matIndex].colors32;

            for (int j = 0; j < 4; j++)
                vertexColors[vertexIndex + j].a = 0;
        }

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private IEnumerator TypeWithFade()
    {
        TMP_TextInfo textInfo = tmpText.textInfo;

        int visibleCharIndex = 0;  // real visible char index in TMP
        int typedCharIndex = 0;    // index into the original fullText with ^ marks

        while (typedCharIndex < fullText.Length && visibleCharIndex < textInfo.characterCount)
        {
            char currentChar = fullText[typedCharIndex];

            if (currentChar == hesitationMarker)
            {
                yield return new WaitForSeconds(hesitationDelay);
            }
            else
            {
                if (textInfo.characterInfo[visibleCharIndex].isVisible)
                {
                    StartCoroutine(FadeCharacter(visibleCharIndex, 0f, 1f, fadeDuration));
                    yield return new WaitForSeconds(delayBetweenChars);
                }

                visibleCharIndex++;
            }

            typedCharIndex++;
        }
    }

    private IEnumerator FadeCharacter(int index, float from, float to, float duration)
    {
        TMP_TextInfo textInfo = tmpText.textInfo;

        int matIndex = textInfo.characterInfo[index].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[index].vertexIndex;
        Color32[] vertexColors = textInfo.meshInfo[matIndex].colors32;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            byte a = (byte)(alpha * 255);

            for (int j = 0; j < 4; j++)
                vertexColors[vertexIndex + j].a = a;

            tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int j = 0; j < 4; j++)
            vertexColors[vertexIndex + j].a = (byte)(to * 255);

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
