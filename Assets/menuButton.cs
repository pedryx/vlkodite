using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using FMODUnity;

public class SpookyMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color32(160, 0, 0, 255);

    [Header("Audio")]
    [SerializeField] private EventReference clickSound; // FMOD event

    public UnityEvent onClickAction;

    private void Start()
    {
        buttonText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.DOColor(hoverColor, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.DOColor(normalColor, 0.2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Play FMOD sound
        if (clickSound.IsNull == false)
        {
            RuntimeManager.PlayOneShot(clickSound);
        }

        Sequence s = DOTween.Sequence();
        s.Append(buttonText.DOColor(Color.white, 0.1f))
         .Append(buttonText.DOColor(hoverColor, 0.1f))
         .OnComplete(() =>
         {
             onClickAction?.Invoke();
         });
    }
}
