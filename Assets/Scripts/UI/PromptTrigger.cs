using UnityEngine;

using DG.Tweening;

[RequireComponent(typeof(Interactable))]
public class PromptTrigger2D : MonoBehaviour
{
    [Header("Prompt Settings")]
    [SerializeField] private string promptMessage = "Interact (E)";

    [Header("Outline Settings")]
    [Tooltip("Child object that has the sprite with outline material")]
    [SerializeField] private SpriteRenderer outlineSpriteRenderer;

    [Tooltip("Duration for outline fade")]
    [SerializeField] private float fadeDuration = 0.2f;

    [Tooltip("Target opacity when visible")]
    [SerializeField] private float visibleOpacity = 1f;

    [Tooltip("Name of the float property controlling the outline opacity")]
    [SerializeField] private string opacityProperty = "Opacity";

    private Material _runtimeMaterial;
    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        if (outlineSpriteRenderer != null)
        {
            _mpb = new MaterialPropertyBlock();
            outlineSpriteRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat("Opacity", 0f);
            outlineSpriteRenderer.SetPropertyBlock(_mpb);
        }

        var interactable = GetComponent<Interactable>();
        interactable.OnInteractionEnabled.AddListener(Interactable_OnInteractionEnabled);
        interactable.OnInteractionDisabled.AddListener(Interactable_OnInteractionDisabled);
    }

    private void Start()
    {
        if (outlineSpriteRenderer != null)
        {
            _mpb = new MaterialPropertyBlock();
            outlineSpriteRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(opacityProperty, 0f);
            outlineSpriteRenderer.SetPropertyBlock(_mpb);
        }
    }

    private void ForceInitialOpacity()
    {
        if (outlineSpriteRenderer != null)
        {
            outlineSpriteRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat("Opacity", 0f);
            outlineSpriteRenderer.SetPropertyBlock(_mpb);
            Debug.Log("Opacity forced to 0 at start.");
        }
    }

    private void FadeOutline(float targetOpacity)
    {
        if (outlineSpriteRenderer == null) return;

        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();

        outlineSpriteRenderer.GetPropertyBlock(_mpb);
        float currentOpacity = _mpb.GetFloat(opacityProperty);

        DOTween.To(() => currentOpacity, x =>
        {
            currentOpacity = x;
            _mpb.SetFloat(opacityProperty, currentOpacity);
            outlineSpriteRenderer.SetPropertyBlock(_mpb);
        }, targetOpacity, fadeDuration);
    }

    private void Interactable_OnInteractionEnabled(Interactable interactable)
    {
        GameManager.Instance.ShowContextPrompt(promptMessage);
        FadeOutline(visibleOpacity);
    }

    private void Interactable_OnInteractionDisabled(Interactable interactable)
    {
        // when player have visible context hint and game xits, returned game manager is null and this gets called
        GameManager.Instance?.HideContextPrimpt();
        FadeOutline(0f);
    }


}
