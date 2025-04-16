using UnityEngine;


/// <summary>
/// Adds quest advancement capability to an game object. When player finishes his interaction with this game object,
/// sub quest will be completed.
/// </summary>
public class QuestAdvancer : MonoBehaviour, IInteractable
{
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
    public InteractionMode InteractionMode { get; private set; }

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

    [HideInInspector]
    public bool IsContinuous => InteractionMode == InteractionMode.Hold;

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        if (InteractionMode != InteractionMode.Hold)
            return;

        if (!interactionThisFrame)
            elapsed = 0.0f;
        interactionThisFrame = false;
    }

    public void Interact(PlayerController player)
    {
        if (!enabled)
            return;

        switch (InteractionMode)
        {
            case InteractionMode.Press:
            {
                ChildController.Instance.FinishSubQuest();
                break;
            }
            case InteractionMode.Hold:
            {
                elapsed += Time.deltaTime;
                interactionThisFrame = true;
                if (elapsed >= Value)
                {
                    elapsed = 0.0f;
                    ChildController.Instance.FinishSubQuest();
                }
                break;
            }
            case InteractionMode.PressMultiple:
            {
                pressCounter++;
                if (pressCounter >= (int)Value)
                {
                    pressCounter = 0;
                    ChildController.Instance.FinishSubQuest();
                }
                break;
            }
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