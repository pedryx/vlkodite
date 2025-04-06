using UnityEngine;
using DG.Tweening;

public class LetterReveal : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private GameObject letterObject; // The parent GameObject holding the UI
    [SerializeField] private CanvasGroup letterCanvasGroup;

    private bool isPlayerNearby = false;
    private bool isLetterVisible = false;

    void Start()
    {
        if (letterCanvasGroup == null && letterObject != null)
        {
            letterCanvasGroup = letterObject.GetComponent<CanvasGroup>();
        }

        if (letterObject != null)
        {
            letterObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!isLetterVisible)
                ShowLetter();
            else
                HideLetter();
        }
    }

    private void ShowLetter()
    {
        if (letterObject == null || letterCanvasGroup == null) return;

        letterObject.SetActive(true);
        letterCanvasGroup.alpha = 0f;
        letterCanvasGroup.interactable = true;
        letterCanvasGroup.blocksRaycasts = true;
        letterCanvasGroup.DOFade(1f, fadeDuration);
        isLetterVisible = true;
    }

    private void HideLetter()
    {
        if (letterObject == null || letterCanvasGroup == null) return;

        letterCanvasGroup.DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                letterObject.SetActive(false);
            });

        letterCanvasGroup.interactable = false;
        letterCanvasGroup.blocksRaycasts = false;
        isLetterVisible = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = false;
            if (isLetterVisible)
            {
                HideLetter();
            }
        }
    }
}
