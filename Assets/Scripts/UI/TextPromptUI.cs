using UnityEngine;
using DG.Tweening;
using TMPro;    // or UnityEngine.UI if you’re using UI.Text

[RequireComponent(typeof(CanvasGroup))]
public class TextPromptUI : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;  // or Text if you’re not using TMP
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup cg;
    private Tween currentTween;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void Show(string message)
    {
        currentTween?.Kill();
        promptText.text = message;
        currentTween = cg
            .DOFade(1f, fadeDuration)
            .OnStart(() =>
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
            });
    }

    public void Hide()
    {
        currentTween?.Kill();
        currentTween = cg
            .DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            });
    }
}
