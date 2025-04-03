using System;

using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private bool isDay = true;
    private float nightTimeElapsed = 0.0f;

    [SerializeField]
    private ChildController child;

    public bool IsDay => isDay;

    public bool IsNight => !isDay;

    [field: SerializeField]
    public float NightDuration { get; private init; } = 5.0f;

    public event EventHandler OnDayBegin;
    public event EventHandler OnNightBegin;

    private void Child_OnAllTasksDone(object sender, EventArgs e)
    {
        isDay = false;
        Debug.Log("Night started.");
        OnNightBegin?.Invoke(this, EventArgs.Empty);
    }

    protected override void Awake()
    {
        base.Awake();

        child.OnAllTasksDone += Child_OnAllTasksDone;
    }

    private void Update()
    {
        if (isDay)
            return;

        nightTimeElapsed += Time.deltaTime;

        if (nightTimeElapsed >= NightDuration)
        {
            isDay = true;
            Debug.Log("Day started.");
            OnDayBegin?.Invoke(this, EventArgs.Empty);
        }
    }
}