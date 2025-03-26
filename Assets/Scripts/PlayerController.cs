using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private PlayerInputActions input;

    /// <summary>
    /// Interactable object in player interaction area. If null there is no target in player area.
    /// </summary>
    private IInteractable interactableTarget;
    /// <summary>
    /// Target movement velocity.
    /// </summary>
    private Vector2 targetVelocity;

    /// <summary>
    /// Item in player's inventory.
    /// </summary>
    public ChildItemScriptableObject Item { get; set; }

    /// <summary>
    /// Time in second which it takes player to reach <see cref="MovementSpeed"/>.
    /// 
    /// When player starts moving, there is a small acceleration window. This determines duration (in seconds) of that window.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Time in second which it takes player to reach its maximum movement speed.\n\n" +
        "When player starts moving, there is a small acceleration window. This determines duration (in seconds) of that window.")]
    public float MovementStartTime { get; private set; } = 0.3f;

    /// <summary>
    /// Time in second which it takes player to stop moving.
    /// 
    /// When player stops moving, there is a small de-acceleration window. This determines duration (in seconds) of that window.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Time in second which it takes player to stop moving.\n\n" +
        "When player stops moving, there is a small de-acceleration window. This determines duration (in seconds) of that window.")]
    public float MovementEndTime { get; private set; } = 0.1f;

    /// <summary>
    /// Maximum movement speed of player in pixels per second.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Maximum movement speed of player in pixels per second.")]
    public float MovementSpeed { get; private set; } = 1.0f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        input = new PlayerInputActions();

        input.Player.Enable();
    }

    private void Update()
    {
        targetVelocity = input.Player.Move.ReadValue<Vector2>().normalized * MovementSpeed;

        if (input.Player.Interact.WasPressedThisFrame() && interactableTarget != null)
            interactableTarget.Interact(this);
    }

    private void FixedUpdate()
    {
        float rate = (targetVelocity.magnitude > 0 ? MovementSpeed / MovementStartTime : MovementSpeed / MovementEndTime);

        Vector2 force = Vector2.ClampMagnitude(targetVelocity - rigidBody.linearVelocity, rate * Time.fixedDeltaTime);
        rigidBody.AddForce(force, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable == null)
            return;

        interactableTarget = interactable;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactableTarget != interactable)
            return;

        interactableTarget = null;
    }
}
