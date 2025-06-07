using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FMODUnity; // Make sure FMOD is set up correctly

public class PaperDropUI : MonoBehaviour
{
    public Image targetImage;
    public float delay = 1f;
    public float dropDistance = 100f;
    public float fadeDuration = 0.6f;
    public float moveDuration = 0.6f;
    public EventReference paperDropSound; // Assign your FMOD event here

    private CanvasGroup canvasGroup;

    void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("PaperDropUI: No targetImage assigned.");
            return;
        }

        // Add or get CanvasGroup for fading
        canvasGroup = targetImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = targetImage.gameObject.AddComponent<CanvasGroup>();
        }

        // Set initial state
        canvasGroup.alpha = 0f;
        RectTransform rt = targetImage.rectTransform;
        rt.anchoredPosition += new Vector2(0, dropDistance);

        // Start animation after delay
        Invoke(nameof(PlayDropAnimation), delay);
    }

    void PlayDropAnimation()
    {
        RectTransform rt = targetImage.rectTransform;

        // Fade in and move down
        canvasGroup.DOFade(1f, fadeDuration);
        rt.DOAnchorPosY(rt.anchoredPosition.y - dropDistance, moveDuration).SetEase(Ease.OutCubic);

        // Play FMOD event
        if (paperDropSound.IsNull == false)
        {
            RuntimeManager.PlayOneShot(paperDropSound);
        }
        else
        {
            Debug.LogWarning("PaperDropUI: No FMOD event assigned.");
        }
    }
}
