using System;

using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Quest
{
    [field: SerializeField]
    public string Description { get; private set; }
    /// <summary>
    /// When player finished his interaction with this quest advancer, quest will be completed.
    /// </summary>
    [field: SerializeField]
    [Tooltip("When player finished his interaction with this quest advancer, quest will be completed.")]
    public QuestAdvancer QuestAdvancer { get; private set; } = null;
    /// <summary>
    /// New position of the child after quest starts.
    /// </summary>
    [field: SerializeField]
    [Tooltip("New position of the child after quest starts.")]
    public Transform ChildPosition { get; private set; } = null;
    /// <summary>
    /// Determine if child should teleport to <see cref="ChildPosition"/> position, otherwise it will walk to it.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Determine if child should teleport to Child Position, otherwise it will walk to it.")]
    public bool Teleport { get; private set; } = false;

    public bool IsStarted { get; private set; } = false;
    public bool IsCompleted { get; private set; } = false;

    /// <summary>
    /// Occur when quest is started.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when quest is started.")]
    public UnityEvent<QuestEventArgs> OnStart { get; private set; } = new();
    /// <summary>
    /// Occur when quest is completed.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when quest is completed.")]
    public UnityEvent<QuestEventArgs> OnDone { get; private set; } = new();

    public Quest(string description)
    {
        Description = description;
    }

    public void Start()
    {
        Debug.Assert(!IsStarted);
        Debug.Assert(!IsCompleted);

        IsStarted = true;
        if (GameManager.Instance.IsDay)
        {
            if (Teleport)
                ChildController.Instance.GetComponent<Transform>().localPosition = ChildPosition.localPosition;
            else
                ChildController.Instance.GetComponent<PathFollow>().Target = ChildPosition;
        }
        OnStart.Invoke(new QuestEventArgs(this));
    }

    public void Complete()
    {
        Debug.Assert(IsStarted);
        Debug.Assert(!IsCompleted);

        IsCompleted = true;
        if (GameManager.Instance.IsDay)
            ChildController.Instance.GetComponent<PathFollow>().Target = null;
        OnDone.Invoke(new QuestEventArgs(this));
    }

    public void Reset()
    {
        IsStarted = false;
        IsCompleted = false;
    }
}

/// <summary>
/// Quest related event arguments.
/// </summary>
/// <param name="Quest">Related quest.</param>
public record QuestEventArgs(Quest Quest);