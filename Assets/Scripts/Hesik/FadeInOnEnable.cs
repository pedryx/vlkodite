using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeInSpriteOnEnable : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayBeforeFade = 1f; // Inspector-adjustable delay

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false; // Hide immediately on Awake
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedFadeIn());
    }

    private System.Collections.IEnumerator DelayedFadeIn()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        spriteRenderer.enabled = true;

        Color c = spriteRenderer.color;
        c.a = 0f;
        spriteRenderer.color = c;

        spriteRenderer.DOFade(1f, fadeDuration).SetUpdate(true); // Fade-in
    }
}
