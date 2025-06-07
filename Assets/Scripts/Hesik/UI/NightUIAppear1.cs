using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class NightUIAppear1 : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float visibleTime = 2f;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ShowUI();
    }

    private void ShowUI()
    {
        currentTween?.Kill();

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
            });
    }
}
