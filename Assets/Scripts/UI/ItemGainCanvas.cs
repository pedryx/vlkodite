using UnityEngine;
using DG.Tweening;

public class ItemGainCanvas : MonoBehaviour
{
    [Tooltip("CanvasGroup attached to your UI Canvas")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Tooltip("Fade duration in seconds")]
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isVisible = false;

    [Tooltip("Other CanvasGroups to fade out when this one appears")]
    [SerializeField] private CanvasGroup[] canvasesToHide;



    private void Start()
    {
        // Ensure canvas starts hidden
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }


    private void OnEnable()
    {
        if (!isVisible)
        {
            ShowCanvas();
        }
    }



    private void ShowCanvas()
    {
        canvasGroup.DOKill();
        transform.DOKill();

        // Scale effect
        transform.localScale = Vector3.one * 1.1f;
        transform.DOScale(1f, fadeDuration).SetEase(Ease.OutBack);

        // Fade this one in
        canvasGroup.DOFade(1f, fadeDuration);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isVisible = true;

        // Fade out all others
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

        // Auto-hide after delay
        float autoHideDelay = 5f; // Change this to how long you want it to stay visible
        DOVirtual.DelayedCall(autoHideDelay, () =>
        {
            if (isVisible) HideCanvas();
        });
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
