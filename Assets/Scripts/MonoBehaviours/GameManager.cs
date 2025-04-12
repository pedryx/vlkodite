using System;

using UnityEngine;
// -60 -10
// 60 80
public class GameManager : Singleton<GameManager>
{
    private PathFinder pathfinder;
    private bool isDay = true;
    private float nightTimeElapsed = 0.0f;

    [SerializeField]
    private GameObject child;

    [SerializeField, Header("PathFinder")]
    private bool showPathFindingGrid = false;
    [SerializeField]
    private Rect pathFinderArea;
    [SerializeField]
    private float pathFinderCellSize = 0.2f;
    [SerializeField]
    private float pathFinderRadius = 0.1f;

    public bool IsDay => isDay;

    public bool IsNight => !isDay;

    [field: SerializeField, Header("")]
    public float NightDuration { get; private set; } = 1000.0f;

    public event EventHandler OnDayBegin;
    public event EventHandler OnNightBegin;

    private void Child_OnAllTasksDone(object sender, EventArgs e)
    {
        isDay = false;
        OnNightBegin?.Invoke(this, EventArgs.Empty);
    }

    protected override void Awake()
    {
        base.Awake();

        OnDayBegin += GameManager_OnDayBegin;
        OnNightBegin += GameManager_OnNightBegin;

        child.GetComponent<ChildController>().OnAllTasksDone += Child_OnAllTasksDone;
        child.GetComponent<WerewolfController>().OnPlayerCaught += GameManager_OnPlayerCaught;

        pathfinder = new PathFinder(pathFinderArea, pathFinderCellSize, pathFinderRadius);
    }

    private void GameManager_OnDayBegin(object sender, EventArgs e)
    {
        Debug.Log("Day started.");
    }

    private void GameManager_OnNightBegin(object sender, EventArgs e)
    {
        Debug.Log("Night started.");
    }

    private void GameManager_OnPlayerCaught(object sender, EventArgs e)
    {
        isDay = true;
        OnDayBegin?.Invoke(this, EventArgs.Empty);
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
        if (!showPathFindingGrid)
            return;

        pathfinder.DrawGizmos();
    }
}