using System;

using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private bool isDay = true;
    /// <summary>
    /// Time elapsed since start of the night.
    /// </summary>
    private float nightTimeElapsed = 0.0f;

    /// <summary>
    /// Determine if the path finding grid should be visualized.
    /// </summary>
    [Header("PathFinder")]
    [field: SerializeField]
    [Tooltip("Determine if path finding grid should be visualized.")]
    public bool ShowPathFindingGrid { get; set; }

    /// <summary>
    /// Determine if current patches should be visualized.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Determine if current patches should be visualized.")]
    public bool ShowPathFindingPatches { get; set; }

    /// <summary>
    /// Bounds of the path finding grid.
    /// </summary>
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

    public bool IsDay => isDay;

    public bool IsNight => !isDay;

    /// <summary>
    /// Night duration in seconds.
    /// </summary>
    [Header("")]
    [field: SerializeField]
    [Tooltip("Night duration in seconds.")]
    public float NightDuration { get; private set; } = 10.0f * 60.0f;

    public PathFinder PathFinder { get; private set; }

    public event EventHandler OnDayBegin;
    public event EventHandler OnNightBegin;

    protected override void Awake()
    {
        base.Awake();

        OnDayBegin += GameManager_OnDayBegin;
        OnNightBegin += GameManager_OnNightBegin;

        ChildController.Instance.OnAllQuestsDone += Child_OnAllQuestsDone;
        WerewolfController.Instance.OnPlayerCaught += Werewolf_OnPlayerCaught;

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
            OnDayBegin?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDrawGizmos()
    {
        if (PathFinder == null || !ShowPathFindingGrid)
            return;

        PathFinder.DrawGizmos();
    }

    private void GameManager_OnDayBegin(object sender, EventArgs e)
    {
        Debug.Log("Day started.");
    }

    private void GameManager_OnNightBegin(object sender, EventArgs e)
    {
        Debug.Log("Night started.");
    }

    private void Werewolf_OnPlayerCaught(object sender, EventArgs e)
    {
        isDay = true;
        OnDayBegin?.Invoke(this, EventArgs.Empty);
    }

    private void Child_OnAllQuestsDone(object sender, EventArgs e)
    {
        isDay = false;
        OnNightBegin?.Invoke(this, EventArgs.Empty);
    }
}