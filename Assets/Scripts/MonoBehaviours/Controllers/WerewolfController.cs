using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(PathFollow))]
public class WerewolfController : Singleton<WerewolfController>
{
    [SerializeField]
    private GameObject werewolfForm;
    [SerializeField]
    private PlayerTrigger visionTrigger;
    [SerializeField]
    private PlayerTrigger catchTrigger;
    [SerializeField]
    private Transform werewolfNight1Spawn;
    [SerializeField]
    private Transform werewolfNight2Spawn;
    [SerializeField]
    private Transform werewolfNight3Spawn;

    private CharacterMovement characterMovement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PathFollow pathFollow;

    /// <summary>
    /// Occur when player is caught by the werewolf.
    /// </summary>
    [Tooltip("Occur when player is caught by the werewolf.")]
    public UnityEvent OnPlayerCaught = new();

    protected override void Awake()
    {
        base.Awake();

        GameManager.Instance.OnDayBegin.AddListener(Instance_OnDayBegin);
        GameManager.Instance.OnNightBegin.AddListener(Instance_OnNightBegin);
        OnPlayerCaught.AddListener(WerewolfController_OnPlayerCaught);

        characterMovement = GetComponent<CharacterMovement>();
        pathFollow = GetComponent<PathFollow>();

        animator = werewolfForm.GetComponent<Animator>();
        spriteRenderer = werewolfForm.GetComponent<SpriteRenderer>();

        Debug.Assert(visionTrigger != null);
        Debug.Assert(catchTrigger != null);
        visionTrigger.OnEnter.AddListener(VisionTrigger_OnEnter);
        catchTrigger.OnEnter.AddListener(CatchTrigger_OnEnter);

        Debug.Assert(werewolfNight1Spawn != null, "Werewolf spawn for first night not specified.");
        Debug.Assert(werewolfNight2Spawn != null, "Werewolf spawn for second night not specified.");
        Debug.Assert(werewolfNight3Spawn != null, "Werewolf spawn for third night not specified.");

        enabled = false;
    }

    private void Update()
    {
        if (!characterMovement.IsMoving())
            spriteRenderer.flipX = characterMovement.GetFacingDirection() == Direction.Left;
        animator.SetBool("IsMoving", !characterMovement.IsMoving());
    }

    private void OnEnable()
    {
        werewolfForm.SetActive(true);

        transform.localPosition = GameManager.Instance.DayNumber switch
        {
            1 => werewolfNight1Spawn.position,
            2 => werewolfNight2Spawn.position,
            3 => werewolfNight3Spawn.position,
            _ => throw new InvalidOperationException(
                $"Spawn position for day {GameManager.Instance.DayNumber} not specified"
            ),
        };
    }

    private void OnDisable()
    {
        werewolfForm.SetActive(false);
        pathFollow.Target = null;
    }

    private void WerewolfController_OnPlayerCaught()
    {
        Debug.Log("Player caught");
    }

    private void Instance_OnDayBegin()
    {
        characterMovement.Move(Vector2.zero);
        enabled = false;
    }

    private void Instance_OnNightBegin()
    {
        enabled = true;
    }

    private void VisionTrigger_OnEnter()
    {
        pathFollow.Target = PlayerController.Instance.transform;
    }

    private void CatchTrigger_OnEnter()
    {
        if (PlayerController.Instance.GodModeActive)
            return;

        OnPlayerCaught.Invoke();
    }
}
