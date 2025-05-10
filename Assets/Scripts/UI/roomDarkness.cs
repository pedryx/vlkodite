using UnityEngine;
using DG.Tweening;

public class RoomDarkness : MonoBehaviour
{
    // temporary fix
    private static bool applicationQuited = false;

    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float fadeDuration = 0.3f;

    private SpriteRenderer spriteRenderer;
    private float originalAlpha;

   

    private void Awake()
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
        if (other.CompareTag(targetTag) && !applicationQuited)
        {
            spriteRenderer.DOFade(originalAlpha, fadeDuration);
        }
    }

    private void OnApplicationQuit()
    {
        applicationQuited = true;
    }
}
