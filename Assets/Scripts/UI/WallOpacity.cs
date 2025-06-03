using UnityEngine;
using DG.Tweening;

public class WallFadeDOTween : MonoBehaviour
{
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
        if (IsRelevantTag(other.tag))
        {
            playerInsideCount++;

            if (playerInsideCount == 1)
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
        if (IsRelevantTag(other.tag))
        {
            playerInsideCount--;

            if (playerInsideCount <= 0)
            {
                playerInsideCount = 0; // Clamp
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].DOFade(originalAlphas[i], fadeDuration);
                }
            }
        }
    }

    private bool IsRelevantTag(string tag)
    {
        return tag == "Player" || tag == "Child";
    }
}
