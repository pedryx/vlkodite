using UnityEngine;
using DG.Tweening;

public class WallFadeDOTween : MonoBehaviour
{
    public string triggerTag = "Player";
    public float fadeDuration = 0.5f;
    public float targetAlpha = 0f;

    private SpriteRenderer[] renderers;
    private float[] originalAlphas;

    private int playerInsideCount = 0;

    private void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalAlphas = new float[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalAlphas[i] = renderers[i].color.a;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag))
        {
            playerInsideCount++;

            if (playerInsideCount == 1) // Only fade if this is the first trigger
            {
                foreach (var r in renderers)
                {
                    if (r != null)
                        r.DOFade(targetAlpha, fadeDuration);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag))
        {
            playerInsideCount--;

            if (playerInsideCount <= 0)
            {
                playerInsideCount = 0; // Clamp to zero
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].DOFade(originalAlphas[i], fadeDuration);
                }
            }
        }
    }
}
