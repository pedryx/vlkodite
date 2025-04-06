using UnityEngine;
using DG.Tweening;

public class roomDarkness : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float fadeDuration = 0.3f;

    private SpriteRenderer spriteRenderer;
    private float originalAlpha;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalAlpha = spriteRenderer.color.a;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            spriteRenderer.DOFade(0f, fadeDuration);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            spriteRenderer.DOFade(originalAlpha, fadeDuration);
        }
    }
}
