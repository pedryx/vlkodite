using DG.Tweening;
using System.Security;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("UI")]
    [SerializeField]
    private TextPromptUI contextPromptUI;
    [SerializeField]
    private QuestLogController questLogController;
    [SerializeField]
    private CinemachineCamera playerCamera;
    [SerializeField]
    private GameObject chaseAudio1;
    [SerializeField]
    private GameObject chaseAudio2;

    /// <summary>
    /// Bounds of the path finding grid.
    /// </summary>
    [Header("PathFinder")]
    [SerializeField]
    [Tooltip("Bounds of the path finding grid.")]
    private Rect pathFinderBounds;
    /// <summary>
    /// Size of cells in the path finding grid.
    /// </summary>
    [SerializeField]
    [Tooltip("Size of cells in path finding grid.")]
    private float pathFinderCellSize = 0.2f;
    /// <summary>
    /// Radius used for physics overlap checks during creation of the path finding grid.
    /// </summary>
    [SerializeField]
    [Tooltip("Radius used for physics overlap checks during creation of the path finding grid.")]
    private float pathFinderRadius = 0.1f;

    /// <summary>
    /// Determine if day is active.
    /// </summary>
    private bool isDay = true;

    /// <summary>
    /// Determine if the path finding grid should be visualized.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Determine if path finding grid should be visualized.")]
    public bool ShowPathFindingGrid { get; set; } = false;
    /// <summary>
    /// Determine if current patches should be visualized.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Determine if current patches should be visualized.")]
    public bool ShowPathFindingPatches { get; set; } = false;

    /// <summary>
    /// Determine if day is active.
    /// </summary>
    public bool IsDay => isDay;

    /// <summary>
    /// Determine if night is active.
    /// </summary>
    public bool IsNight => !isDay;

    public int DayNumber { get; private set; } = 1;

    public PathFinder PathFinder { get; private set; }

    // Following events should be properties, but we can't change that now because they have a lot events linked on
    // them now.

    public UnityEvent OnDayBegin = new();
    public UnityEvent OnNightBegin = new();
    public UnityEvent OnDayNightSwitch = new();

    // Following events are redundant, but they allow more comfortable usage from the inspector.

    [Header("Day/Night cycle")]
    [field: SerializeField]
    public UnityEvent OnFirstDayBegin { get; private set; } = new();
    [field: SerializeField]
    public UnityEvent OnSecondDayBegin { get; private set; } = new();
    [field: SerializeField]
    public UnityEvent OnThirdDayBegin { get; private set; } = new();
    [field: SerializeField]
    public UnityEvent OnFirstNightBegin { get; private set; } = new();
    [field: SerializeField]
    public UnityEvent OnSecondNightBegin { get; private set; } = new();
    [field: SerializeField]
    public UnityEvent OnThirdNightBegin { get; private set; } = new();

    protected override void Awake()
    {
        base.Awake();

        OnDayBegin.AddListener(GameManager_OnDayBegin);
        OnNightBegin.AddListener(GameManager_OnNightBegin);
        QuestManager.Instance.OnTransitionQuestDone.AddListener(TransitionQuest_OnDone);

        PathFinder = new PathFinder(pathFinderBounds, pathFinderCellSize, pathFinderRadius);

        Debug.Assert(questLogController != null, "Quest log is not assigned.");
        Debug.Assert(playerCamera != null, "Player camera is not assigned.");
        Debug.Assert(chaseAudio1 != null, "Chase audio 1 is not assigned.");
        Debug.Assert(chaseAudio2 != null, "Chase audio 2 is not assigned.");
    }

    private void OnDrawGizmos()
    {
        if (PathFinder == null || !ShowPathFindingGrid)
            return;

        PathFinder.DrawGizmos();
    }

    public void RestartNight()
    {
        Debug.Assert(IsNight);

        questLogController.Restart();
        QuestManager.Instance.Current.Restart();

        WerewolfController.Instance.enabled = false;
        PlayerController.Instance.Respawn();

        var cameras = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        foreach (var camera in cameras)
            camera.Priority = CameraZoneSwitcher.defaultPriority;
        playerCamera.Priority = CameraZoneSwitcher.activePriority;

        chaseAudio1.SetActive(false);
        chaseAudio2.SetActive(false);

        OnNightBegin.Invoke();
    }

    /// <summary>
    /// Toggle between day and night.
    /// </summary>
    /// <returns>True if switched to day otherwise false.</returns>
    public bool SwitchDayNight()
    {
        isDay = !isDay;

        if (isDay)
            OnDayBegin.Invoke();
        else
            OnNightBegin.Invoke();

        return isDay;
    }

    public void ShowContextPrompt(string promptText)
        => contextPromptUI.Show(promptText);

    public void HideContextPrompt()
        => contextPromptUI.Hide();

    public void SwitchScene(string sceneName)
        => SceneManager.LoadScene(sceneName);

    private void GameManager_OnDayBegin()
    {
        Debug.Log("Day started.");
        DayNumber++;

        OnDayNightSwitch.Invoke();

        switch(DayNumber)
        {
            case 1:
                OnFirstDayBegin.Invoke();
                break;
            case 2:
                WerewolfController.Instance.ScriptedCatch = false;
                OnSecondDayBegin.Invoke();
                break;
            case 3:
                OnThirdDayBegin.Invoke();
                break;
        }
    }

    private void GameManager_OnNightBegin()
    {
        Debug.Log("Night started.");
        OnDayNightSwitch.Invoke();

        switch (DayNumber)
        {
            case 1:
                WerewolfController.Instance.ScriptedCatch = true;
                OnFirstNightBegin.Invoke();
                break;
            case 2:
                WerewolfController.Instance.ScriptedCatch = false;
                OnSecondNightBegin.Invoke();
                break;
            case 3:
                OnThirdNightBegin.Invoke();
                break;
        }

    }

    private void TransitionQuest_OnDone(QuestEventArgs e)
        => SwitchDayNight();
}