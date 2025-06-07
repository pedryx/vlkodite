using UnityEngine;
using DG.Tweening; // Requires DOTween

[RequireComponent(typeof(CanvasGroup))]
public class FadeOutAndDisable : MonoBehaviour
{
    [SerializeField] private float delay = 2f;
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        canvasGroup.alpha = 1f; // Ensure fully visible
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        CancelInvoke();
        Invoke(nameof(FadeAndDisable), delay);
    }

    private void FadeAndDisable()
    {
        canvasGroup.DOFade(0f, fadeDuration)
            .OnStart(() =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            })
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}
