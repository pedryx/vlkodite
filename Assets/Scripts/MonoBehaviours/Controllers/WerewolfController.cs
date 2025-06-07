using System;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;
using FMOD.Studio;


[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(PathFollow))]
public class WerewolfController : Singleton<WerewolfController>
{
    #region Components
    [Header("Components")]
    [SerializeField]
    private GameObject werewolfForm;
    [SerializeField]
    private PlayerTrigger visionTrigger;
    [SerializeField]
    private PlayerTrigger catchTrigger;
    [SerializeField]
    private PlayerTrigger grabStartTrigger;
    [SerializeField]
    private Animator werewolfAnimator;
    [SerializeField]
    private Animator eyesAnimator;
    [SerializeField]
    private SpriteRenderer werewolfSpriteRenderer;
    [SerializeField]
    private SpriteRenderer eyesSpriteRenderer;

    private CharacterMovement characterMovement;
    private PathFollow pathFollow;
    #endregion
    #region Scripted catch mechanic
    /// <summary>
    /// When enabled <see cref="ScriptedAccelaration"/> is applied to the werewolf. This ensures that the werewolf
    /// catches the player during the scripted chase sequence.
    /// </summary>
    [Header("Scripted catch mechanic")]
    [SerializeField]
    [Tooltip("When enabled, scripted acceleration is applied to the werewolf. This ensures that the werewolf " +
        "catches the player during the scripted chase sequence.")]
    private bool scriptedCatch = false;

    // Following property is under scripted catch field, so its appear near each other in the inspector.

    /// <summary>
    /// Accelarattion of the werewolf when <see cref="ScriptedCatch"/> is enabled."/>
    /// </summary>
    [field: SerializeField]
    [Tooltip("Acceleration of the werewolf when ScriptedCatch is enabled.")]
    public float ScriptedAccelaration { get; private set; } = 0.1f;

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
                StandartSpeed = characterMovement.Speed;
            else
                characterMovement.Speed = StandartSpeed;
        }
    }
    #endregion
    #region Kitchen encounter
    /// <summary>
    /// How many times will kitchen animation be repeated when werewolf notices player.
    /// </summary>
    [Header("Kiten encounter")]
    [SerializeField]
    [Tooltip("How many times will kitchen animation be repeated when werewolf notices player.")]
    private int noticeKitchenIdleRepeats = 1;
    /// <summary>
    /// How mant times did kitchen idle animation was repeated.
    /// </summary>
    private int noticeKitchenIdleRepeatCounter = 0;
    [SerializeField]
    private EventReference kitchenNoticeEvent;
    [SerializeField]
    private GameObject kitchenNoticeObjectA;
    [SerializeField]
    private GameObject kitchenNoticeObjectB;
    #endregion
    #region Night spawns
    [Header("Night spawns")]
    [SerializeField]
    private Transform werewolfNight1Spawn;
    [SerializeField]
    private Transform werewolfNight2Spawn;
    [SerializeField]
    private Transform werewolfNight3Spawn;
    #endregion
    #region Events
    /// <summary>
    /// Occur when player is caught by the werewolf.
    /// </summary>
    [Header("Events")]
    [Tooltip("Occur when player is caught by the werewolf.")]
    public UnityEvent OnPlayerCaught = new();

    /// <summary>
    /// Occur when werewolf completes his reverse transformation.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when werewolf completes his reverse transformation.")]
    public UnityEvent OnReverseTransformDone { get; private set; }

    /// <summary>
    /// Occur when werewolf starts chasing the player.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when werewolf starts chasing the player.")]
    public UnityEvent OnChaseStart { get; private set; } = new();
    #endregion
    #region Transform night 2
    /// <summary>
    /// How many times will transform middle animation be repeated when werewolf notices player.
    /// </summary>
    [Header("Transform night 2")]
    [SerializeField]
    [Tooltip("How many times will transform middle animation be repeated when werewolf notices player.")]
    private int TransformMiddleRepeats = 1;
    /// <summary>
    /// How mant times did transform middle animation was repeated.
    /// </summary>
    private int TransformMiddleRepeatCounter = 0;
    #endregion

    /// <summary>
    /// Standart werewolf speed. This is used to restore the speed when werewolf speed needs to be temporary changed.
    /// </summary>
    public float StandartSpeed { get; set; } = 0.0f;
    /// <summary>
    /// Determine if player is inside catch trigger.
    /// </summary>
    private bool IsplayerInCatchTrigger = false;
    /// <summary>
    /// Determine if move animation should be forced even if werewolf is not moving.
    /// </summary>
    private bool forceMoveAnimation = false;
    /// <summary>
    /// Determine if player entered werewolf's vision trigger during current night.
    /// </summary>
    private bool hasSeenPlayer = false;

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
        catchTrigger.OnEnter.AddListener(() => IsplayerInCatchTrigger = true);
        catchTrigger.OnExit.AddListener(() => IsplayerInCatchTrigger = false);

        #region animations
        var animations = werewolfForm.GetComponent<WerewolfAnimationEvents>();
        animations.OnCatchTriggerFrame.AddListener(Animations_InCatchFrame);
        animations.OnLastCatchFrame.AddListener(Animations_OnLastCatchFrame);
        animations.OnLastKitchenNoticeFrame.AddListener(Animations_OnLastKitchenNoticeFrame);
        animations.OnLastKitchenIdleFrame.AddListener(Animations_OnLastKitchenIdleFrame);
        animations.OnLastTransformMiddleFrame.AddListener(Animations_OnLastTransformMiddleFrame);
        animations.OnLastTransformFrame.AddListener(Animations_OnTransformDone);
        #endregion

        Debug.Assert(werewolfNight1Spawn != null, "Werewolf spawn for first night not specified.");
        Debug.Assert(werewolfNight2Spawn != null, "Werewolf spawn for second night not specified.");
        Debug.Assert(werewolfNight3Spawn != null, "Werewolf spawn for third night not specified.");

        StandartSpeed = characterMovement.Speed;
        enabled = false;
    }

    private void Update()
    {
        if (!characterMovement.IsNotMoving())
        {
            bool isFacingLeft = characterMovement.GetFacingDirection() == Direction.Left;

            Vector3 scale = transform.localScale;
            scale.x = (isFacingLeft ? -1.0f : 1.0f) * Mathf.Abs(scale.x);
            transform.localScale = scale;

            forceMoveAnimation = false;
        }
        werewolfAnimator.SetBool("IsMoving", !characterMovement.IsNotMoving() || forceMoveAnimation);
        eyesAnimator.SetBool("IsMoving", !characterMovement.IsNotMoving() || forceMoveAnimation);

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
        hasSeenPlayer = false;

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
        if (GameManager.Instance.DayNumber == 3)
        {
            enabled = false;
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

    #region Grab mechanic
    private void GrabStartTrigger_OnEnter()
    {
        if (PlayerController.Instance.GodModeActive)
            return;

        werewolfAnimator.SetBool("IsGrabing", true);
        eyesAnimator.SetBool("IsGrabing", true);
        characterMovement.Speed = 0;
    }

    private void Animations_InCatchFrame()
    {
        if (!IsplayerInCatchTrigger)
            return;

        if (GameManager.Instance.DayNumber == 2)
        {
            GameManager.Instance.RestartNight();
        }
        else
        {
            QuestManager.Instance.Current.TransitionQuest.Complete();
            OnPlayerCaught.Invoke();
        }
    }

    private void Animations_OnLastCatchFrame()
    {
        werewolfAnimator.SetBool("IsGrabing", false);
        eyesAnimator.SetBool("IsGrabing", false);
        characterMovement.Speed = StandartSpeed;
    }
    #endregion

    private void VisionTrigger_OnEnter()
    {
        if (hasSeenPlayer)
            return;
        hasSeenPlayer = true;

        if (GameManager.Instance.DayNumber == 2)
        {
            werewolfAnimator.Play("TransformEvent");
            eyesAnimator.Play("TransformEventEyes");
        }
    }

    #region Kitchen encounter
    private void Animations_OnLastKitchenIdleFrame()
    {
        if (!hasSeenPlayer)
            return;

        if (noticeKitchenIdleRepeatCounter == noticeKitchenIdleRepeats)
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
        forceMoveAnimation = true;
        OnChaseStart.Invoke();
    }
    #endregion
    #region Transform night 2
    private void Animations_OnLastTransformMiddleFrame()
    {
        if (!hasSeenPlayer)
            return;

        if (TransformMiddleRepeatCounter == TransformMiddleRepeats)
        {
            werewolfAnimator.Play("TransformEvent", -1, 0.0f);
            eyesAnimator.Play("TransformEventEyes", -1, 0.0f);
            TransformMiddleRepeatCounter = 0;
        }
        TransformMiddleRepeatCounter++;
    }

    private void Animations_OnTransformDone()
    {
        werewolfAnimator.Play("RunSide", -1, 0.0f);
        eyesAnimator.Play("RunSide", -1, 0.0f);
        pathFollow.Target = PlayerController.Instance.transform;
        QuestManager.Instance.Current.ChildQuestQueue.Quests[0].Complete();
        forceMoveAnimation = true;
        OnChaseStart.Invoke();
    }
    #endregion
}
