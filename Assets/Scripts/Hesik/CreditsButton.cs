using UnityEngine;
using DG.Tweening;

public class CreditsButton : MonoBehaviour
{
    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private CanvasGroup creditsGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (creditsGroup != null)
            creditsGroup.alpha = 0f;

        if (creditsCanvas != null)
            creditsCanvas.SetActive(false);
    }

    public void OnCreditsPressed()
    {
        if (creditsCanvas != null && creditsGroup != null)
        {
            creditsCanvas.SetActive(true);
            creditsGroup.alpha = 0f; // Reset
            creditsGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine);
        }
    }

    public void OnCloseCredits()
    {
        if (creditsCanvas != null && creditsGroup != null)
        {
            creditsGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutSine)
                .OnComplete(() => creditsCanvas.SetActive(false));
        }
    }
}
