using System;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;
using FMOD.Studio;


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
    private bool forceMoveAfterKitchenScene = false;
    /// <summary>
    /// How many times will kitchen animation be repeated when werewolf notices player.
    /// </summary>
    [SerializeField]
    [Tooltip("How many times will kitchen animation be repeated when werewolf notices player.")]
    private int noticeKitchenIdleRepeat = 1;
    private int noticeKitchenIdleRepeatCounter = 0;

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

    [SerializeField]
    private Animator werewolfAnimator;
    [SerializeField]
    private Animator eyesAnimator;
    [SerializeField]
    private SpriteRenderer werewolfSpriteRenderer;
    [SerializeField]
    private SpriteRenderer eyesSpriteRenderer;

    [SerializeField]
    private EventReference kitchenNoticeEvent;
   
    [SerializeField]
    private GameObject kitchenNoticeObjectA;

    [SerializeField]
    private GameObject kitchenNoticeObjectB;


    private CharacterMovement characterMovement;
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
    /// Occur when werewolf completes his reverse transformation.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when werewolf completes his reverse transformation.")]
    public UnityEvent OnReverseTransformDone { get; private set; }

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

    /// <summary>
    /// Start reverse transformation of the werewolf. This starts playing the animation in which werewolf transform
    /// back into his human form. You can detect end of this animation by <see cref="OnReverseTransformDone"/> event.
    /// </summary>
    [Tooltip("Start reverse transformation of the werewolf. This starts playing the animation in which werewolf " +
        "transform back into his human form. You can detect end of this animation by OnReverseTransformDone event.")]
    public void PlayReverseTransformEvent()
    {
        werewolfAnimator.Play("ReverseTransformEvent");
        eyesAnimator.Play("ReverseTransformEvent");
    }

    protected override void Awake()
    {
        base.Awake();

        GameManager.Instance.OnDayBegin.AddListener(Instance_OnDayBegin);
        GameManager.Instance.OnNightBegin.AddListener(Instance_OnNightBegin);
        OnPlayerCaught.AddListener(WerewolfController_OnPlayerCaught);

        characterMovement = GetComponent<CharacterMovement>();
        pathFollow = GetComponent<PathFollow>();

        Debug.Assert(visionTrigger != null);
        Debug.Assert(catchTrigger != null);
        visionTrigger.OnEnter.AddListener(VisionTrigger_OnEnter);

        grabStartTrigger.OnEnter.AddListener(GrabStartTrigger_OnEnter);
        catchTrigger.OnEnter.AddListener(CatchTrigger_OnEnter);
        catchTrigger.OnExit.AddListener(CatchTrigger_OnExit);
        werewolfAnimationEvents = werewolfForm.GetComponent<WerewolfAnimationEvents>();
        werewolfAnimationEvents.OnCatchTriggerFrame.AddListener(WerewolfAnimationEvents_InCatchFrame);
        werewolfAnimationEvents.OnLastCatchFrame.AddListener(WerewolfAnimationEvents_AfterLastFrame);
        werewolfAnimationEvents.OnLastKitchenNoticeFrame.AddListener(WerewolfAnimationEvents_OnLastKitchenNoticeFrame);
        werewolfAnimationEvents.OnLastKitchenIdleFrame.AddListener(WerewolfAnimationEvents_OnLastKitchenIdleFrame);
        werewolfAnimationEvents.OnReverseTransformDone.AddListener(() => OnReverseTransformDone.Invoke());

        Debug.Assert(werewolfNight1Spawn != null, "Werewolf spawn for first night not specified.");
        Debug.Assert(werewolfNight2Spawn != null, "Werewolf spawn for second night not specified.");
        Debug.Assert(werewolfNight3Spawn != null, "Werewolf spawn for third night not specified.");

        enabled = false;
    }

    private void Update()
    {
        if (!characterMovement.IsNotMoving())
        {
            werewolfSpriteRenderer.flipX = characterMovement.GetFacingDirection() == Direction.Left;
            eyesSpriteRenderer.flipX = werewolfSpriteRenderer.flipX;
            forceMoveAfterKitchenScene = false;
        }
        werewolfAnimator.SetBool("IsMoving", !characterMovement.IsNotMoving() || forceMoveAfterKitchenScene);
        eyesAnimator.SetBool("IsMoving", !characterMovement.IsNotMoving() || forceMoveAfterKitchenScene);

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

        if (GameManager.Instance.DayNumber == 1)
        {
            werewolfAnimator.Play("IdleKitchen", -1, 0.0f);
            eyesAnimator.Play("IdleKitchenEyes", -1, 0.0f);
        }
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

        werewolfAnimator.SetBool("IsGrabing", true);
        eyesAnimator.SetBool("IsGrabing", true);
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
        werewolfAnimator.SetBool("IsGrabing", false);
        eyesAnimator.SetBool("IsGrabing", false);
        characterMovement.Speed = speed;

        Debug.Log("werewolf grab end");
    }

    private void VisionTrigger_OnEnter()
    {
        if (visionTriggered)
            return;
        visionTriggered = true;
    }

    private void WerewolfAnimationEvents_OnLastKitchenIdleFrame()
    {
        if (!visionTriggered)
            return;

        if (noticeKitchenIdleRepeatCounter == noticeKitchenIdleRepeat)
        {
            werewolfAnimator.Play("NoticeKitchen", -1, 0.0f);
            eyesAnimator.Play("NoticeKitchenEyes", -1, 0.0f);
        }

        noticeKitchenIdleRepeatCounter++;
    }

    private void WerewolfAnimationEvents_OnLastKitchenNoticeFrame()
    {
        // Play the FMOD sound
        RuntimeManager.PlayOneShot(kitchenNoticeEvent);

        // Disable both GameObjects if assigned
        if (kitchenNoticeObjectA != null)
            kitchenNoticeObjectA.SetActive(false);

        if (kitchenNoticeObjectB != null)
            kitchenNoticeObjectB.SetActive(false);

        // Continue werewolf behavior
        werewolfAnimator.Play("RunSide", -1, 0.0f);
        eyesAnimator.Play("RunSide", -1, 0.0f);
        pathFollow.Target = PlayerController.Instance.transform;
        QuestManager.Instance.Current.ChildQuestQueue.ActiveQuest.Complete();
        forceMoveAfterKitchenScene = true;
    }


}
