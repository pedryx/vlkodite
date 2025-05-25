using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageFadeOut : MonoBehaviour
{
    [SerializeField] private Image imageToFade;
    [SerializeField] private float fadeDuration = 1f;

    public void FadeOut()
    {
        if (imageToFade != null)
        {
            imageToFade.DOFade(0f, fadeDuration);
        }
    }
}
