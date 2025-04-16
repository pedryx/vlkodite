using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement))]
public class PlayerController : Singleton<PlayerController>
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CharacterMovement characterMovement;
    private GameInputActions input;

    /// <summary>
    /// Determine if debug mode is toggled on. In debug mode player have useful debugging capabilities, like
    /// super-speed, task skip, etc.
    /// </summary>
    private bool debugMode = false;
    private Vector3 spawnPosition;
    private float characterSpeed;

    /// <summary>
    /// Interactable object in player interaction area. If null there is no target in player area.
    /// </summary>
    private IInteractable interactableTarget;

    /// <summary>
    /// Maximum movement speed of player in pixels per second during super-speed mode.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Maximum movement speed of player in pixels per second during super speed mode.")]
    public float SuperMovementSpeed { get; private set; } = 1.0f;

    protected override void Awake()
    {
        base.Awake();

        characterMovement = GetComponent<CharacterMovement>();
        characterSpeed = characterMovement.Speed;

        input = new GameInputActions();
        input.Player.Enable();
        input.Debug.ToggleDebug.Enable();
        input.Debug.ToggleDebug.performed += ToggleDebug_Performed;

        spawnPosition = transform.position;
        WerewolfController.Instance.OnPlayerCaught += Werewolf_OnPlayerCaught;
        GameManager.Instance.OnNightBegin += Instance_OnNightBegin;

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        // update movement
        characterMovement.Move(input.Player.Move.ReadValue<Vector2>());
        if (input.Debug.SuperSpeed.WasPressedThisFrame())
            characterMovement.Speed = SuperMovementSpeed;
        if (input.Debug.SuperSpeed.WasReleasedThisFrame())
            characterMovement.Speed = characterSpeed;

        // update animation
        if (!characterMovement.IsMoving())
        {
            Direction facingDirection = characterMovement.GetFacingDirection();
            Debug.Assert(facingDirection != Direction.None);

            spriteRenderer.flipX = facingDirection == Direction.Left;
            animator.SetFloat("Direction", (float)facingDirection);
        }
        animator.SetBool("IsMoving", !characterMovement.IsMoving());

        // update interactions
        if (interactableTarget != null)
        {
            if (interactableTarget.IsContinuous && input.Player.Interact.IsPressed())
                interactableTarget.Interact(this);
            else if (!interactableTarget.IsContinuous && input.Player.Interact.WasPressedThisFrame())
                interactableTarget.Interact(this);
        }
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
        transform.localPosition = spawnPosition;
    }

    private void Instance_OnNightBegin(object sender, System.EventArgs e)
    {
        transform.localPosition = spawnPosition;
    }
}
