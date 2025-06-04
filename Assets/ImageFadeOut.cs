using UnityEngine;
using DG.Tweening;
using FMODUnity;

public class ScreenFader : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup blackOverlay;
    [SerializeField] private float fadeToBlackDuration = 1f;
    [SerializeField] private float blackScreenDuration = 1f;
    [SerializeField] private float fadeFromBlackDuration = 1f;

    [Header("Audio")]
    [SerializeField] private EventReference sleepSound;

    [Header("Player Control")]
    [SerializeField] private CharacterMovement characterMovement; // Assign in Inspector

    private float originalSpeed;

    private void Start()
    {
        blackOverlay.alpha = 0f;
    }

    private void OnEnable()
    {
        FadeToBlackAndBack();
    }

    public void FadeToBlackAndBack()
    {
        RuntimeManager.PlayOneShot(sleepSound);

        if (characterMovement != null)
        {
            originalSpeed = characterMovement.Speed;
            characterMovement.Speed = 0f;
        }

        Sequence fadeSequence = DOTween.Sequence();

        fadeSequence.Append(blackOverlay.DOFade(1f, fadeToBlackDuration))
                    .AppendInterval(blackScreenDuration)
                    .Append(blackOverlay.DOFade(0f, fadeFromBlackDuration))
                    .OnComplete(() =>
                    {
                        if (characterMovement != null)
                            characterMovement.Speed = originalSpeed;

                        gameObject.SetActive(false);
                    });
    }

    public void FadeToBlack()
    {
        RuntimeManager.PlayOneShot(sleepSound);
        blackOverlay.DOFade(1f, fadeToBlackDuration);
    }

    public void FadeFromBlack()
    {
        blackOverlay.DOFade(0f, fadeFromBlackDuration);
    }
}
