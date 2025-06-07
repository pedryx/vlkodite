using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using FMODUnity;

public class SpriteSlideRight : MonoBehaviour
{
    [Header("Slide Settings")]
    [SerializeField] private float slideDistance = 2f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Ease slideEase = Ease.OutQuad;

    [Header("FMOD")]
    [SerializeField] private EventReference slideStartSound;

    [Header("Optional Event When Slide Ends")]
    public UnityEvent onSlideComplete;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void SlideRight()
    {
        if (slideStartSound.IsNull == false)
        {
            RuntimeManager.PlayOneShot(slideStartSound, transform.position);
        }

        Vector3 targetPosition = originalPosition + Vector3.right * slideDistance;

        transform.DOMove(targetPosition, slideDuration)
                 .SetEase(slideEase)
                 .OnComplete(() => onSlideComplete?.Invoke());
    }

    public void ResetPosition()
    {
        transform.position = originalPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 from = Application.isPlaying ? originalPosition : transform.position;
        Vector3 to = from + Vector3.right * slideDistance;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawWireSphere(to, 0.1f);
    }
}
