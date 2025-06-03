using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(PathFollow))]
public class WerewolfController : Singleton<WerewolfController>
{
    /// <summary>
    /// When enabled <see cref="ScriptedAccelaration"/> is applied to the werewolf. This ensures that the werewolf
    /// catches the player during the scripted chase sequence.
    /// </summary>
    [SerializeField]
    [Tooltip("When enabled, scripted acceleration is applied to the werewolf. This ensures that the werewolf " +
        "catches the player during the scripted chase sequence.")]
    private bool scriptedCatch = false;
    private float standartSpeed;
    /// <summary>
    /// Determine if player is inside catch trigger.
    /// </summary>
    private bool playerInCatchTrigger = false;
    private float speed;

    // Following property is under scripted catch field, so its appear near each other in the inspector.

    /// <summary>
    /// Accelarattion of the werewolf when <see cref="ScriptedCatch"/> is enabled."/>
    /// </summary>
    [field: SerializeField]
    [Tooltip("Acceleration of the werewolf when ScriptedCatch is enabled.")]
    public float ScriptedAccelaration { get; private set; } = 0.1f;

    [SerializeField]
    private GameObject werewolfForm;
    [SerializeField]
    private PlayerTrigger visionTrigger;
    [SerializeField]
    private PlayerTrigger catchTrigger;
    [SerializeField]
    private PlayerTrigger grabStartTrigger;
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
    private WerewolfAnimationEvents werewolfAnimationEvents;

    /// <summary>
    /// Determine if player entered werewolf's vision trigger during current night.
    /// </summary>
    private bool visionTriggered = false;

    /// <summary>
    /// Occur when player is caught by the werewolf.
    /// </summary>
    [Tooltip("Occur when player is caught by the werewolf.")]
    public UnityEvent OnPlayerCaught = new();

    /// <summary>
    /// When enabled <see cref="ScriptedAccelaration"/> is applied to the werewolf. This ensures that the werewolf
    /// catches the player during the scripted chase sequence.
    /// </summary>
    public bool ScriptedCatch
    {
        get => scriptedCatch;
        set
        {
            scriptedCatch = value;
            if (value)
                standartSpeed = characterMovement.Speed;
            else
                characterMovement.Speed = standartSpeed;
        }
    }

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

        grabStartTrigger.OnEnter.AddListener(GrabStartTrigger_OnEnter);
        catchTrigger.OnEnter.AddListener(CatchTrigger_OnEnter);
        catchTrigger.OnExit.AddListener(CatchTrigger_OnExit);
        werewolfAnimationEvents = werewolfForm.GetComponent<WerewolfAnimationEvents>();
        werewolfAnimationEvents.InCatchFrame.AddListener(WerewolfAnimationEvents_InCatchFrame);
        werewolfAnimationEvents.AfterLastFrame.AddListener(WerewolfAnimationEvents_AfterLastFrame);

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

        if (ScriptedCatch)
            characterMovement.Speed += ScriptedAccelaration * Time.deltaTime;
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
        visionTriggered = false;
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
        if (visionTriggered)
            return;

        pathFollow.Target = PlayerController.Instance.transform;
        QuestManager.Instance.Current.ChildQuestQueue.ActiveQuest.Complete();
        visionTriggered = true;
    }

    private void CatchTrigger_OnEnter()
    {
        playerInCatchTrigger = true;
    }

    private void CatchTrigger_OnExit()
    {
        playerInCatchTrigger = false;
    }

    private void GrabStartTrigger_OnEnter()
    {
        if (PlayerController.Instance.GodModeActive)
            return;

        animator.SetBool("IsGrabing", true);
        speed = characterMovement.Speed;
        characterMovement.Speed = 0;
    }

    private void WerewolfAnimationEvents_InCatchFrame()
    {
        if (!playerInCatchTrigger)
            return;

        QuestManager.Instance.Current.TransitionQuest.Complete();
        OnPlayerCaught.Invoke();

        Debug.Log("player got catch");
    }

    private void WerewolfAnimationEvents_AfterLastFrame()
    {
        animator.SetBool("IsGrabing", false);
        characterMovement.Speed = speed;

        Debug.Log("werewolf grab end");
    }
}
