using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class QuestQueue
{
    [field: SerializeField]
    private List<Quest> quests = new();

    private int questQueueIndex = 0;

    public Quest ActiveQuest => quests[questQueueIndex];

    /// <summary>
    /// Contains active quests.
    /// </summary>
    public IReadOnlyList<Quest> Quests => quests;

    /// <summary>
    /// Occur when any quest in quest queue starts.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when any quest in quest queue starts.")]
    public UnityEvent<QuestQueueEventArgs> OnQuestStart { get; private set; } = new();
    /// <summary>
    /// Occur wheny any quest in quest queue is finished.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur wheny any quest in quest queue is finished.")]
    public UnityEvent<QuestQueueEventArgs> OnQuestDone { get; private set; } = new();
    /// <summary>
    /// Occur when last quest in quest queue is finished.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when last quest in quest queue is finished.")]
    public UnityEvent<QuestQueueEventArgs> OnAllQuestsDone { get; private set; } = new();

    /// <summary>
    /// Determine if all quests in queue are done.
    /// </summary>
    public bool AreAllQuestsDone => questQueueIndex >= quests.Count;

    /// <summary>
    /// Index of the current quest in the queue.
    /// </summary>
    public int QuestIndex => questQueueIndex;

    public void Start()
    {
        foreach (var quest in quests)
        {
            quest.OnStart.AddListener(Quest_OnStart);
            quest.OnDone.AddListener(Quest_OnDone);
        }

        ActiveQuest.Start();
    }

    public void Reset()
    {
        questQueueIndex = 0;
        foreach (var quest in quests)
        {
            quest.Reset();
        }
    }

    private void Quest_OnStart(QuestEventArgs e)
    {
        OnQuestStart.Invoke(new QuestQueueEventArgs(this, e.Quest));
    }

    private void Quest_OnDone(QuestEventArgs e)
    {
        Debug.Assert(questQueueIndex < quests.Count);
        Debug.Assert(e.Quest == ActiveQuest);

        OnQuestDone.Invoke(new QuestQueueEventArgs(this, e.Quest));

        questQueueIndex++;
        if (questQueueIndex == quests.Count)
        {
            OnAllQuestsDone.Invoke(new QuestQueueEventArgs(this, e.Quest));
            return;
        }

        ActiveQuest.Start();
    }
}

/// <summary>
/// Quest related event arguments.
/// </summary>
/// <param name="Quest">Related quest.</param>
public record QuestQueueEventArgs(QuestQueue Queue, Quest Quest);