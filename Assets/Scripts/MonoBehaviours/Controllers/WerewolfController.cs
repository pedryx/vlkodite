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
    private bool forceMove = false;
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
    /// Occur when werewolf starts chasing the player.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when werewolf starts chasing the player.")]
    public UnityEvent OnChaseStart { get; private set; } = new();

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

        GameManager.Instance.OnDayBegin.AddListener(GameManager_OnDayBegin);
        GameManager.Instance.OnNightBegin.AddListener(GameManager_OnNightBegin);
        OnPlayerCaught.AddListener(() => Debug.Log("Player get caught by the werewolf."));
        OnChaseStart.AddListener(() => Debug.Log("Werewolf started chasing the player."));

        characterMovement = GetComponent<CharacterMovement>();
        pathFollow = GetComponent<PathFollow>();

        Debug.Assert(visionTrigger != null);
        Debug.Assert(catchTrigger != null);
        visionTrigger.OnEnter.AddListener(VisionTrigger_OnEnter);

        grabStartTrigger.OnEnter.AddListener(GrabStartTrigger_OnEnter);
        catchTrigger.OnEnter.AddListener(() => playerInCatchTrigger = true);
        catchTrigger.OnExit.AddListener(() => playerInCatchTrigger = false);

        var animations = werewolfForm.GetComponent<WerewolfAnimationEvents>();
        animations.OnCatchTriggerFrame.AddListener(Animations_InCatchFrame);
        animations.OnLastCatchFrame.AddListener(Animations_OnLastCatchFrame);
        animations.OnLastKitchenNoticeFrame.AddListener(Animations_OnLastKitchenNoticeFrame);
        animations.OnLastKitchenIdleFrame.AddListener(Animations_OnLastKitchenIdleFrame);
        animations.OnReverseTransformDone.AddListener(() => OnReverseTransformDone.Invoke());
        animations.OnLastTransformFrame.AddListener(Animations_OnTransformDone);

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
            forceMove = false;
        }
        werewolfAnimator.SetBool("IsMoving", !characterMovement.IsNotMoving() || forceMove);
        eyesAnimator.SetBool("IsMoving", !characterMovement.IsNotMoving() || forceMove);

        if (ScriptedCatch && pathFollow.Target != null)
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
        if (GameManager.Instance.DayNumber == 2)
        {
            werewolfAnimator.Play("TransformMiddle", -1, 0.0f);
            eyesAnimator.Play("NoEyes", -1, 0.0f);
        }
    }

    private void OnDisable()
    {
        werewolfForm.SetActive(false);
        pathFollow.Target = null;
    }

    private void GameManager_OnDayBegin()
    {
        characterMovement.Move(Vector2.zero);
        enabled = false;
    }

    private void GameManager_OnNightBegin()
    {
        enabled = true;
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

    private void Animations_InCatchFrame()
    {
        if (!playerInCatchTrigger)
            return;

        QuestManager.Instance.Current.TransitionQuest.Complete();
        OnPlayerCaught.Invoke();
    }

    private void Animations_OnLastCatchFrame()
    {
        werewolfAnimator.SetBool("IsGrabing", false);
        eyesAnimator.SetBool("IsGrabing", false);
        characterMovement.Speed = speed;
    }

    private void VisionTrigger_OnEnter()
    {
        if (visionTriggered)
            return;
        visionTriggered = true;

        if (GameManager.Instance.DayNumber == 2)
        {
            werewolfAnimator.Play("TransformEvent");
            eyesAnimator.Play("TransformEventEyes");
        }
    }

    private void Animations_OnLastKitchenIdleFrame()
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

    private void Animations_OnLastKitchenNoticeFrame()
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
        while (!QuestManager.Instance.Current.ChildQuestQueue.AreAllQuestsDone)
            QuestManager.Instance.Current.ChildQuestQueue.ActiveQuest.Complete();
        forceMove = true;
        OnChaseStart.Invoke();
    }

    private void Animations_OnTransformDone()
    {
        werewolfAnimator.Play("RunSide", -1, 0.0f);
        eyesAnimator.Play("RunSide", -1, 0.0f);
        pathFollow.Target = PlayerController.Instance.transform;
        QuestManager.Instance.Current.ChildQuestQueue.ActiveQuest.Complete();
        forceMove = true;
        OnChaseStart.Invoke();
    }
}
