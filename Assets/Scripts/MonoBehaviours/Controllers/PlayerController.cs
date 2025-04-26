using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement))]
public class PlayerController : Singleton<PlayerController>
{
    /// <summary>
    /// Interactable objects in player interaction area.
    /// </summary>
    private readonly HashSet<Interactable> interactableTargets = new();

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
        WerewolfController.Instance.OnPlayerCaught.AddListener(Werewolf_OnPlayerCaught);
        GameManager.Instance.OnNightBegin.AddListener(GameManager_OnNightBegin);

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
        foreach (var interactible in interactableTargets)
        {
            if (interactible.IsContinuous && input.Player.Interact.IsPressed())
                interactible.Interact();
            else if (!interactible.IsContinuous && input.Player.Interact.WasPressedThisFrame())
                interactible.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Interactable>(out var interactable))
            return;

        if (interactableTargets.Add(interactable) && interactable.InteractionEnabled)
            interactable.OnInteractionEnabled.Invoke(interactable);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Interactable>(out var interactable))
            return;
        
        if (interactableTargets.Remove(interactable) && interactable.InteractionEnabled)
            interactable.OnInteractionDisabled.Invoke(interactable);
    }

    /// <summary>
    /// Determine if player is currently near the interactiible game object.
    /// </summary>
    public bool IsNear(Interactable interactable)
        => interactableTargets.Contains(interactable);

    private void ToggleDebug_Performed(InputAction.CallbackContext context)
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

    private void Werewolf_OnPlayerCaught()
    {
        transform.localPosition = spawnPosition;
    }

    private void GameManager_OnNightBegin()
    {
        transform.localPosition = spawnPosition;
    }
}
