using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Player can interact with every game object with this component.
/// </summary>
public class Interactable : MonoBehaviour
{
    /// <summary>
    /// Determine if player could interact with the game object.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Determine if player could interact with the game object.")]
    private bool interactionEnabled = true;
    /// <summary>
    /// Time elapsed since hold interaction begin. Valid only for <see cref="InteractionMode.Hold"/>.
    /// </summary>
    private float elapsed = 0.0f;
    /// <summary>
    /// How many times interaction key was pressed. Valid only for <see cref="InteractionMode.PressMultiple"/>.
    /// </summary>
    private int pressCounter = 0;
    /// <summary>
    /// Determine if there was interaction during this frame.
    /// </summary>
    private bool interactionThisFrame = false;

    [field: SerializeField]
    public InteractionMode InteractionMode { get; private set; } = InteractionMode.Press;

    /// <summary>
    /// Meaning of this property depends on <see cref="InteractionMode"/>.
    /// 
    /// * <see cref="InteractionMode.Press"/>: No meaning.
    /// * <see cref="InteractionMode.Hold"/>: Duration how long the interaction key needs to be hold down in seconds.
    /// * <see cref="InteractionMode.PressMultiple"/>: How many time the button needs to be pressed.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Meaning of this property depends on InteractionMode.\n\n" +
        "- InteractionMode.Press: No meaning.\n" +
        "- InteractionMode.Hold: Duration how long the interaction key needs to be hold down in seconds.\n" +
        "- InteractionMode.PressMultiple: How many time the button needs to be pressed.\n")]
    public float Value { get; private set; } = 3.0f;

    /// <summary>
    /// Determines whether the interaction is continuous (i.e., it occurs every frame while the interaction key is held
    /// down), or instant (i.e., it occurs only once when the key is initially pressed).
    /// </summary>
    public bool IsContinuous => InteractionMode == InteractionMode.Hold;

    /// <summary>
    /// Determine if player could interact with the game object.
    /// </summary>
    public bool InteractionEnabled
    {
        get => interactionEnabled;
        set
        {
            if (interactionEnabled != value && PlayerController.Instance.IsNear(this))
            {
                if (value)
                    OnInteractionEnabled.Invoke(this);
                else
                    OnInteractionDisabled.Invoke(this);
            }

            interactionEnabled = value;
        }
    }
    
    /// <summary>
    /// Occur when interaction with the game object is enabled.
    /// </summary>
    [Tooltip("Occur when interaction with the game object is enabled.")]
    public UnityEvent<Interactable> OnInteractionEnabled = new();
    /// <summary>
    /// Occur when interaction with the game object is disabled.
    /// </summary>
    [Tooltip("Occur when interaction with the game object is disabled.")]
    public UnityEvent<Interactable> OnInteractionDisabled = new();
    /// <summary>
    /// Occur when player completes interaction with the game object.
    /// </summary>
    [Tooltip("Occur when player completes interaction with the game object.")]
    public UnityEvent<Interactable> OnInteract = new();

    private void Update()
    {
        if (InteractionMode != InteractionMode.Hold)
            return;

        if (!interactionThisFrame)
            elapsed = 0.0f;
        interactionThisFrame = false;
    }

    /// <summary>
    /// Interact with the game object.
    /// </summary>
    public void Interact()
    {
        if (!interactionEnabled || !PlayerController.Instance.IsNear(this))
            return;

        switch (InteractionMode)
        {
            case InteractionMode.Press:
                OnInteract?.Invoke(this);
                break;
            case InteractionMode.Hold:
                elapsed += Time.deltaTime;
                interactionThisFrame = true;
                if (elapsed >= Value)
                {
                    elapsed = 0.0f;
                    OnInteract?.Invoke(this);
                }
                break;
            case InteractionMode.PressMultiple:
                pressCounter++;
                if (pressCounter >= (int)Value)
                {
                    pressCounter = 0;
                    OnInteract?.Invoke(this);
                }
                break;
        }
    }
}

public enum InteractionMode
{
    /// <summary>
    /// Interaction key has to be pressed in order to finish the interaction. Value property is not used by this
    /// interaction mode.
    /// </summary>
    Press,
    /// <summary>
    /// Interaction key needs to be hold down for a certain time in order to finish the interaction. Value property
    /// represent the duration how long the interaction key needs to be hold down in seconds.
    /// </summary>
    Hold,
    /// <summary>
    /// Interaction key needs to be pressed multiple times in order to finish the task. Value property represent
    /// how many time the button needs to be pressed.
    /// </summary>
    PressMultiple,
}