using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Determine if debug mode is toggled on. In debug mode player have useful debugging capabilities, like
    /// super-speed, task skip, etc.
    /// </summary>
    private bool debugMode = false;
    private Vector2 spawnPosition;
    private FacingDirection facingDirection;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private GameInputActions input;

    [SerializeField]
    private WerewolfController werewolf;

    /// <summary>
    /// Interactable object in player interaction area. If null there is no target in player area.
    /// </summary>
    private IInteractable interactableTarget;
    /// <summary>
    /// Target movement velocity.
    /// </summary>
    private Vector2 velocity;

    /// <summary>
    /// Item in player's inventory.
    /// </summary>
    public ChildItemScriptableObject Item { get; set; }

    /// <summary>
    /// Maximum movement speed of player in pixels per second during super-speed mode.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Maximum movement speed of player in pixels per second during super speed mode.")]
    public float SuperMovementSpeed { get; private set; } = 1.0f;

    /// <summary>
    /// Maximum movement speed of player in pixels per second.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Maximum movement speed of player in pixels per second.")]
    public float MovementSpeed { get; private set; } = 1.0f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        input = new GameInputActions();

        spawnPosition = rigidBody.position;

        input.Player.Enable();
        input.Debug.ToggleDebug.Enable();
        input.Debug.ToggleDebug.performed += ToggleDebug_Performed;

        werewolf.OnPlayerCaught += Werewolf_OnPlayerCaught;
        GameManager.Instance.OnNightBegin += Instance_OnNightBegin;

        animator.Play("Idle");
        animator.SetFloat("FaceDirection", (float)facingDirection);
    }

    private void Update()
    {
        // update velocity
        float speed = input.Debug.SuperSpeed.IsPressed() ? SuperMovementSpeed : MovementSpeed;
        velocity = input.Player.Move.ReadValue<Vector2>().normalized * speed;
        
        // update animator
        if (velocity.magnitude > 1e-2f)
        {
            if (Mathf.Abs(velocity.x) > 1e-2f)
                facingDirection = FacingDirection.Side;
            else if (velocity.y > 0.0f)
                facingDirection = FacingDirection.Up;
            else
                facingDirection = FacingDirection.Down;

            spriteRenderer.flipX = facingDirection == FacingDirection.Side && velocity.x < 0.0f;
            animator.SetFloat("FaceDirection", (float)facingDirection);
        }

        animator.Play(velocity.magnitude > 1e-2f ? "Walk" : "Idle");

        // update interactions
        if (input.Player.Interact.WasPressedThisFrame() && interactableTarget != null)
            interactableTarget.Interact(this);
    }

    private void FixedUpdate()
    {
        rigidBody.position += velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<IInteractable>(out var interactable))
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

    private void ToggleDebug_Performed(InputAction.CallbackContext obj)
    {
        if (debugMode)
        {
            input.Debug.Disable();
            input.Debug.ToggleDebug.Enable();
            debugMode = false;
            Debug.Log("Debug mode off.");
        }
        else
        {
            input.Debug.Enable();
            debugMode = true;
            Debug.Log("Debug mode on.");
        }
    }

    private void Werewolf_OnPlayerCaught(object sender, System.EventArgs e)
    {
        rigidBody.position = spawnPosition;
    }

    private void Instance_OnNightBegin(object sender, System.EventArgs e)
    {
        rigidBody.position = spawnPosition;
    }

    private enum FacingDirection
    {
        Down,
        Up,
        Side,
    }
}
