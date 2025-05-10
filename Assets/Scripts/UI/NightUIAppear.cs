using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class NightUIAppear : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float visibleTime = 2f;

    [Header("Other Canvases To Hide")]
    [SerializeField] private CanvasGroup[] otherCanvases;

    private CanvasGroup canvasGroup;
    private Tween currentTween;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        ShowUI(); // Automatically show when object becomes active
    }

    private void ShowUI()
    {
        currentTween?.Kill();

        // Fade out other canvases
        foreach (CanvasGroup other in otherCanvases)
        {
            if (other != null)
                other.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    other.interactable = false;
                    other.blocksRaycasts = false;
                });
        }

        currentTween = canvasGroup
            .DOFade(1f, fadeDuration)
            .OnStart(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            })
            .OnComplete(() =>
            {
                Invoke(nameof(HideUI), visibleTime);
            });
    }

    private void HideUI()
    {
        currentTween?.Kill();

        currentTween = canvasGroup
            .DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                // Restore other canvases
                foreach (CanvasGroup other in otherCanvases)
                {
                    if (other != null)
                    {
                        other.DOFade(1f, fadeDuration).OnStart(() =>
                        {
                            other.interactable = true;
                            other.blocksRaycasts = true;
                        });
                    }
                }
            });
    }
}
