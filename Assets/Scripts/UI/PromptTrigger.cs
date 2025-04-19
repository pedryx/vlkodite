using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class PromptTrigger2D : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextPromptUI promptUI;

    [Header("Prompt Settings")]
    [SerializeField] private string promptMessage = "Interact";
    [SerializeField] private string playerTag = "Player";

    [Header("Outline Settings")]
    [Tooltip("Child object that has the sprite with outline material")]
    [SerializeField] private SpriteRenderer outlineSpriteRenderer;

    [Tooltip("Duration for outline fade")]
    [SerializeField] private float fadeDuration = 0.2f;

    [Tooltip("Target opacity when visible")]
    [SerializeField] private float visibleOpacity = 1f;

    [Tooltip("Name of the float property controlling the outline opacity")]
    [SerializeField] private string opacityProperty = "Opacity";

    private bool _playerInside = false;
    private Material _runtimeMaterial;

    private void Reset()
    {
        var col2d = GetComponent<Collider2D>();
        col2d.isTrigger = true;
    }

    private void Awake()
    {
        if (outlineSpriteRenderer != null)
        {
            _mpb = new MaterialPropertyBlock();
            outlineSpriteRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat("Opacity", 0f);
            outlineSpriteRenderer.SetPropertyBlock(_mpb);
        }
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



    private MaterialPropertyBlock _mpb;

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


    private void Update()
    {
        if (_playerInside && Input.GetKeyDown(KeyCode.E))
        {
            promptUI.Hide();
            FadeOutline(0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        _playerInside = true;
        promptUI.Show(promptMessage);
        FadeOutline(visibleOpacity);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        _playerInside = false;
        promptUI.Hide();
        FadeOutline(0f);
    }
}
