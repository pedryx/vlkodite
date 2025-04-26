using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class QuestEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private Image crossOutLine; // Drag the line here in the prefab
    private float originalLineWidth;
    bool isCompleted = false;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        originalLineWidth = crossOutLine.rectTransform.sizeDelta.x;
        // Start line at 0 width
        crossOutLine.rectTransform.sizeDelta = new Vector2(0f, crossOutLine.rectTransform.sizeDelta.y);
        crossOutLine.gameObject.SetActive(false);
    }
    public void SetText(string text)
    {
        questText.text = text;
        crossOutLine.gameObject.SetActive(false);
    }

    public void Complete(Action onComplete)
    {
        crossOutLine.gameObject.SetActive(true);

        // Reset line to 0 width
        RectTransform lineRect = crossOutLine.rectTransform;
        lineRect.sizeDelta = new Vector2(0f, lineRect.sizeDelta.y);

        // Animate line width to original
        DOTween.Sequence()
            .Append(lineRect.DOSizeDelta(new Vector2(originalLineWidth, lineRect.sizeDelta.y), 0.4f))
            .AppendInterval(0.2f)
            .AppendCallback(() => {
                // Fade out both line + text
                crossOutLine.DOFade(0f, 0.3f);
                questText.DOFade(0f, 0.3f).OnComplete(() => onComplete?.Invoke());
            });
    }

    public void CompleteQuest()
    {
        if (isCompleted) return;

        isCompleted = true;

        // Activate line and animate it
        crossOutLine.gameObject.SetActive(true);

        // Animate width from 0 to original width
        crossOutLine.rectTransform.DOSizeDelta(
            new Vector2(originalLineWidth, crossOutLine.rectTransform.sizeDelta.y),
            0.4f
        ).SetEase(Ease.OutQuad);

        // Fade out the whole entry
        canvasGroup.DOFade(0f, 0.5f).SetDelay(0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false); // or destroy if you prefer
        });
    }

}
