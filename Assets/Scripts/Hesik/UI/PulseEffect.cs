using UnityEngine;
using DG.Tweening;

public class PulseEffect : MonoBehaviour
{
    [Tooltip("How big the pulse should be (e.g. 1.05 = 5% bigger)")]
    [SerializeField] private float pulseScale = 1.05f;

    [Tooltip("How long one pulse takes (up + down)")]
    [SerializeField] private float pulseDuration = 1f;

    [Tooltip("Should it start pulsing on Awake?")]
    [SerializeField] private bool autoStart = true;

    private Tween pulseTween;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;

        if (autoStart)
            StartPulse();
    }

    public void StartPulse()
    {
        StopPulse(); // just in case

        pulseTween = transform
            .DOScale(originalScale * pulseScale, pulseDuration / 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void StopPulse()
    {
        if (pulseTween != null && pulseTween.IsActive())
        {
            pulseTween.Kill();
            transform.localScale = originalScale;
        }
    }
}
