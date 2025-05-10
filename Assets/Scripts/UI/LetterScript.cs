using UnityEngine;
using DG.Tweening;

public class LetterScript : MonoBehaviour
{
    [Tooltip("CanvasGroup attached to your UI Canvas")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Tooltip("Fade duration in seconds")]
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isVisible = false;

    [Tooltip("Other CanvasGroups to fade out when this one appears")]
    [SerializeField] private CanvasGroup[] canvasesToHide;

    private void Awake()
    {
        ShowCanvas(); // Show automatically right when the scene loads
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isVisible)
        {
            HideCanvas();
        }
    }

    private void ShowCanvas()
    {
        canvasGroup.DOKill();
        transform.DOKill();

        // Reset scale and alpha instantly before animation
        transform.localScale = Vector3.one * 1.1f;
        canvasGroup.alpha = 0f;

        // Animate
        transform.DOScale(1f, fadeDuration).SetEase(Ease.OutBack);
        canvasGroup.DOFade(1f, fadeDuration);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isVisible = true;

        // Fade out others
        foreach (var cg in canvasesToHide)
        {
            if (cg != null)
            {
                cg.DOKill();
                cg.DOFade(0f, fadeDuration);
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }


    private void HideCanvas()
    {
        canvasGroup.DOKill();
        transform.DOKill();

        // Fade and reset this one
        canvasGroup.DOFade(0f, fadeDuration);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        isVisible = false;

        // Fade other canvases back in
        foreach (var cg in canvasesToHide)
        {
            if (cg != null)
            {
                cg.DOKill();
                cg.DOFade(1f, fadeDuration);
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }
    }
}
