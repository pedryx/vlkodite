using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [Header("UI")]
    [SerializeField]
    private TextPromptUI contextPromptUI;
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
    /// Time elapsed since start of the night.
    /// </summary>
    private float nightTimeElapsed = 0.0f;

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
    /// Night duration in seconds.
    /// </summary>
    [Header("Day/Night cycle")]
    [field: SerializeField]
    [Tooltip("Night duration in seconds.")]
    public float NightDuration { get; private set; } = 5.0f * 60.0f;

    /// <summary>
    /// Determine if day is active.
    /// </summary>
    public bool IsDay => isDay;

    /// <summary>
    /// Determine if night is active.
    /// </summary>
    public bool IsNight => !isDay;

    public PathFinder PathFinder { get; private set; }

    public UnityEvent OnDayBegin = new();
    public UnityEvent OnNightBegin = new();

    protected override void Awake()
    {
        base.Awake();

        OnDayBegin.AddListener(GameManager_OnDayBegin);
        OnNightBegin.AddListener(GameManager_OnNightBegin);
        QuestManager.Instance.OnAllQuestsDone.AddListener(QuestManager_OnAllQuestsDone);
        WerewolfController.Instance.OnPlayerCaught.AddListener(Werewolf_OnPlayerCaught);

        PathFinder = new PathFinder(pathFinderBounds, pathFinderCellSize, pathFinderRadius);
    }

    private void Update()
    {
        if (isDay)
            return;

        nightTimeElapsed += Time.deltaTime;

        if (nightTimeElapsed >= NightDuration)
        {
            isDay = true;
            OnDayBegin?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (PathFinder == null || !ShowPathFindingGrid)
            return;

        PathFinder.DrawGizmos();
    }

    public void ShowContextPrompt(string promptText) => contextPromptUI.Show(promptText);

    public void HideContextPrimpt() => contextPromptUI.Hide();

    private void GameManager_OnDayBegin()
    {
        Debug.Log("Day started.");
    }

    private void GameManager_OnNightBegin()
    {
        Debug.Log("Night started.");
    }

    private void Werewolf_OnPlayerCaught()
    {
        isDay = true;
        OnDayBegin.Invoke();
    }

    private void QuestManager_OnAllQuestsDone()
    {
        isDay = false;
        OnNightBegin.Invoke();
    }
}