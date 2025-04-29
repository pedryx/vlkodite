using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class QuestManager : Singleton<QuestManager>
{
    private int questNotDoneCounter = 0;

    /// <summary>
    /// Contains active quests.
    /// </summary>
    [SerializeField]
    [Tooltip("Contains active quests.")]
    private List<Quest> quests = new();

    /// <summary>
    /// Quest queue with child tasks.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Quest queue with child tasks.")]
    public QuestQueue ChildQuestQueue { get; private set; }

    /// <summary>
    /// Contains active quests.
    /// </summary>
    public IReadOnlyList<Quest> Quests => quests;

    /// <summary>
    /// Occur when any quest is started.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when any quest is started.")]
    public UnityEvent<QuestEventArgs> OnQuestStart { get; private set; } = new();
    /// <summary>
    /// Occur when any quest is finished.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when any quest is finished.")]
    public UnityEvent<QuestEventArgs> OnQuestDone { get; private set; } = new();

    /// <summary>
    /// Occur when all quest and quest queues are done.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when all quest and quest queues are done.")]
    public UnityEvent OnAllQuestsDone { get; private set; } = new();

    protected  override void Awake()
    {
        base.Awake();
        GameManager.Instance.OnDayBegin.AddListener(GameManager_OnDayBegin);

        ChildQuestQueue.OnQuestStart.AddListener(QuestQueue_OnQuestStart);
        ChildQuestQueue.OnQuestDone.AddListener(QuestQueue_OnQuestDone);
        ChildQuestQueue.OnAllQuestsDone.AddListener(QuestQueue_OnAllQuestDone);
        foreach (var quest in quests)
        {
            quest.OnStart.AddListener(Quest_OnStart);
            quest.OnDone.AddListener(Quest_OnDone);
        }
    }

    private void Start()
    {
        Debug.Assert(questNotDoneCounter == 0);
        ChildQuestQueue.Start();
        questNotDoneCounter++;
        foreach (var quest in quests)
        {
            quest.Start();
            questNotDoneCounter++;
        }
    }

    public void AddQuest(Quest quest)
    {
        Debug.Assert(questNotDoneCounter > 0);

        questNotDoneCounter++;
        quests.Add(quest);
        quest.Start();
    }

    private void GameManager_OnDayBegin()
    {
        ChildQuestQueue.Reset();
        foreach (var quest in quests)
        {
            quest.Reset();
        }

        Debug.Assert(questNotDoneCounter == 0);
        ChildQuestQueue.Start();
        questNotDoneCounter++;
        foreach (var quest in quests)
        {
            quest.Start();
            questNotDoneCounter++;
        }
    }

    private void QuestQueue_OnQuestStart(QuestQueueEventArgs e)
    {
        OnQuestStart.Invoke(new QuestEventArgs(e.Quest));
    }
        
    private void QuestQueue_OnQuestDone(QuestQueueEventArgs e)
    {
        OnQuestDone.Invoke(new QuestEventArgs(e.Quest));
    }

    private void QuestQueue_OnAllQuestDone(QuestQueueEventArgs e)
    {
        Debug.Assert(questNotDoneCounter > 0);
        questNotDoneCounter--;
        if (questNotDoneCounter == 0)
            OnAllQuestsDone.Invoke();
    }

    private void Quest_OnStart(QuestEventArgs e)
    {
        OnQuestStart.Invoke(new QuestEventArgs(e.Quest));
    }

    private void Quest_OnDone(QuestEventArgs e)
    {
        OnQuestDone.Invoke(new QuestEventArgs(e.Quest));

        Debug.Assert(questNotDoneCounter > 0);

        questNotDoneCounter--;
        if (questNotDoneCounter == 0)
            OnAllQuestsDone.Invoke();
    }
}