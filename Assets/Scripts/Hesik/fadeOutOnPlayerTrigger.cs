using UnityEngine;
using DG.Tweening;
using FMODUnity;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class FadeOutOnPlayerTrigger : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("FMOD")]
    [SerializeField] private EventReference disappearSound;

    [Header("Player Tag")]
    [SerializeField] private string playerTag = "Player";

    private SpriteRenderer spriteRenderer;
    private bool hasFaded = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            Debug.LogWarning("Collider2D is not set as trigger. Setting it to trigger.");
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasFaded) return;

        if (other.CompareTag(playerTag))
        {
            hasFaded = true;
            FadeOutAndPlaySound();
        }
    }

    private void FadeOutAndPlaySound()
    {
        // Play FMOD sound
        if (disappearSound.IsNull == false)
        {
            RuntimeManager.PlayOneShot(disappearSound, transform.position);
        }

        // Fade sprite
        spriteRenderer.DOFade(0f, fadeDuration).SetUpdate(true);
    }
}
