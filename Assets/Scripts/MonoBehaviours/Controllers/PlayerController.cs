using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CapsuleCollider2D))]
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
    private CapsuleCollider2D capsuleCollider;

    /// <summary>
    /// Determine if debug mode is toggled on. In debug mode player have useful debugging capabilities, like
    /// super-speed, task skip, etc.
    /// </summary>
    private bool debugMode = false;
    private Vector3 spawnPosition;
    private float characterSpeed;
    private float sprintAccumulator = 0.0f;
    private Vector3 nightSpawnPosition;

    /// <summary>
    /// Maximum movement speed of player in pixels per second during super-speed mode.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Maximum movement speed of player in pixels per second during super speed mode.")]
    public float SuperMovementSpeed { get; private set; } = 1.0f;

    /// <summary>
    /// Maximum movement speef of player in pixels per second during sprint.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Maximum movement speef of player in pixels per second during sprint.")]
    public float SprintSpeed { get; private set; } = 12.0f;

    /// <summary>
    /// How long can player maximally sprint in seconds.
    /// </summary>
    [field: SerializeField]
    [Tooltip("How long can player maximally sprint in seconds.")]
    public float MaxSprintDuration { get; private set; } = 3.0f;

    /// <summary>
    /// How long has player to wait before he can sprint again, if he just stopped sprinting.
    /// </summary>
    [field: SerializeField]
    [Tooltip("How long has player to wait before he can sprint again, if he just stopped sprinting.")]
    public float SprintCooldown { get; private set; } = 3.0f;

    /// <summary>
    /// Determine if god mode is active.
    /// </summary>
    public bool GodModeActive { get; private set; } = false;

    public bool IsRunning { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();

        characterMovement = GetComponent<CharacterMovement>();
        characterSpeed = characterMovement.Speed;
        sprintAccumulator = SprintCooldown;

        input = new GameInputActions();
        input.Player.Enable();
        #region Debug input
        input.Debug.ToggleDebug.Enable();
        input.Debug.ToggleDebug.performed += ToggleDebug_Performed;
        input.Debug.ToggleGodMode.performed += ToggleGodMode_Performed;
        input.Debug.FinishQuest.performed += FinishQuest_Performed;
        input.Debug.SwitchDayNight.performed += SwitchDayNight_Performed;
        #endregion

        spawnPosition = transform.position;
        WerewolfController.Instance.OnPlayerCaught.AddListener(Werewolf_OnPlayerCaught);
        GameManager.Instance.OnNightBegin.AddListener(GameManager_OnNightBegin);

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        // update movement
        characterMovement.Move(input.Player.Move.ReadValue<Vector2>());
        if (input.Debug.SuperSpeed.WasPressedThisFrame())
            characterMovement.Speed = SuperMovementSpeed;
        if (input.Debug.SuperSpeed.WasReleasedThisFrame())
            characterMovement.Speed = characterSpeed;
        if (!GodModeActive)
        {
            sprintAccumulator += Time.deltaTime;

            if (!IsRunning && input.Player.Sprint.WasPressedThisFrame() && sprintAccumulator >= SprintCooldown)
            {
                characterMovement.Speed = SprintSpeed;
                IsRunning = true;
                sprintAccumulator = 0.0f;
            }
            if (IsRunning && (input.Player.Sprint.WasReleasedThisFrame() || sprintAccumulator >= MaxSprintDuration))
            {
                characterMovement.Speed = characterSpeed;
                IsRunning = false;
                sprintAccumulator = 0.0f;
            }
        }

        // update animation
        if (!characterMovement.IsNotMoving())
        {
            Direction facingDirection = characterMovement.GetFacingDirection();
            Debug.Assert(facingDirection != Direction.None);

            spriteRenderer.flipX = facingDirection == Direction.Left;
            animator.SetFloat("Direction", (float)facingDirection);
        }
        animator.SetBool("IsMoving", !characterMovement.IsNotMoving());
        animator.SetBool("IsRunning", IsRunning);

        // update interactions
        // We are making shallow copy, because from interactibles from the interactableTargets could be erased during
        // iteration of following loop which could lead to collection modified exception.
        var currentInteractibleTargets = new HashSet<Interactable>(interactableTargets);
        foreach (var interactible in currentInteractibleTargets)
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

    /// <summary>
    /// Resets player position to the position where player was standing at the start of the night.
    /// </summary>
    public void Respawn()
        => transform.localPosition = nightSpawnPosition;

    private void Werewolf_OnPlayerCaught()
        => transform.localPosition = spawnPosition;

    private void GameManager_OnNightBegin()
        => nightSpawnPosition = transform.localPosition;

    #region Debug events
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

    private void ToggleGodMode_Performed(InputAction.CallbackContext context)
    {
        GodModeActive = !GodModeActive;
        capsuleCollider.enabled = !GodModeActive;
        Debug.Log($"GodMode {(GodModeActive ? "on" : "off")}");
    }

    private void FinishQuest_Performed(InputAction.CallbackContext context)
    {
        foreach (var quest in QuestManager.Instance.Current.AllQuests)
        {
            if (!quest.IsCompleted)
            {
                quest.Complete();
                return;
            }
        }

        if (!QuestManager.Instance.Current.TransitionQuest.IsCompleted)
        {
            QuestManager.Instance.Current.TransitionQuest.Complete();
            return;
        }

        Debug.LogWarning("All quests are complete.");
    }

    private void SwitchDayNight_Performed(InputAction.CallbackContext context)
    {
        foreach (var quest in QuestManager.Instance.Current.AllQuests)
        {
            if (!quest.IsCompleted)
                quest.Complete();
        }

        if (!QuestManager.Instance.Current.TransitionQuest.IsCompleted)
            QuestManager.Instance.Current.TransitionQuest.Complete();
    }
    #endregion
}
