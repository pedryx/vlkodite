using UnityEngine;
using DG.Tweening;
using FMODUnity;

[RequireComponent(typeof(CanvasGroup))]
public class NightUIAppear : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float visibleTime = 2f;
    [SerializeField] private float startDelay = 0f; // New delay before appearing

    [Header("Other Canvases To Hide")]
    [SerializeField] private CanvasGroup[] otherCanvases;

    private CanvasGroup canvasGroup;
    private Tween currentTween;

    [Header("FMOD Event (Optional)")]
    [SerializeField] private StudioEventEmitter fmodEmitter; // Optional FMOD sound to play with UI


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
        DOVirtual.DelayedCall(startDelay, ShowUI);
    }

    private void ShowUI()
    {
        currentTween?.Kill();

        // Play FMOD sound if assigned
        if (fmodEmitter != null)
        {
            fmodEmitter.Play();
        }

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
