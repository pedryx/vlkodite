using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

public class QuestManager : Singleton<QuestManager>
{
    private readonly Dictionary<(int, bool), Quests> questsMap;
    private readonly (int, bool) lastTimeBlock = (4, true);

    private int currentDayNumber = 1;
    private bool isDay = true;

    #region Quests for all days and nights
    [field: SerializeField]
    public Quests Day1 { get; private set; } = new();

    [field: SerializeField]
    public Quests Night1 { get; private set; } = new();
    [field: SerializeField]
    public Quests Day2 { get; private set; } = new();

    [field: SerializeField]
    public Quests Night2 { get; private set; } = new();
    [field: SerializeField]
    public Quests Day3 { get; private set; } = new();

    [field: SerializeField]
    public Quests Night3 { get; private set; } = new();
    #endregion

    public Quests Current => questsMap[(currentDayNumber, isDay)];

    /// <summary>
    /// Occur when any quest is started. This occur for all quests from any day and night.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when any quest is started. This occur for all quests from any day and night.")]
    public UnityEvent<QuestEventArgs> OnQuestStart { get; private set; } = new();
    /// <summary>
    /// Occur when any quest is finished. This occur for all quests from any day and night.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when any quest is finished. This occur for all quests from any day and night.")]
    public UnityEvent<QuestEventArgs> OnQuestDone { get; private set; } = new();

    /// <summary>
    /// Occur when all quest and quest queues are done. Transition quest will begin after this event. This occur for
    /// all quests from any day and night.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occur when all quest and quest queues are done. Transition quest will begin after this event. This " +
        "occur for all quests from any day and night.")]
    public UnityEvent OnAllQuestsDone { get; private set; } = new();

    /// <summary>
    /// Occur after new day/night starts and quests of that day/night are initialized.
    /// </summary>
    [Tooltip("Occur after new day/night starts and quests of that day/night are initialized.")]
    public UnityEvent OnQuestsInitialized { get; private set; } = new();

    /// <summary>
    /// Occur when a transition quest from any day or night is completed.
    /// </summary>
    [Tooltip("Occur when a transition quest from any day or night is completed.")]
    public UnityEvent<QuestEventArgs> OnTransitionQuestDone { get; private set; } = new();

    public QuestManager()
    {
        questsMap = new Dictionary<(int, bool), Quests>()
        {
            { (1, true), Day1 },
            { (1, false), Night1 },
            { (2, true), Day2 },
            { (2, false), Night2 },
            { (3, true), Day3 },
            { (3, false), Night3 },
            // TODO: game over
        };
    }

    protected  override void Awake()
    {
        base.Awake();

        Activate(Current);
        GameManager.Instance.OnDayNightSwitch.AddListener(GameManager_OnDayNightSwitch);
    }

    private void Start()
    {
        Current.Start();
    }

    private void Activate(Quests quests)
    {
        quests.OnQuestStart.AddListener(Quests_OnQuestStart);
        quests.OnQuestDone.AddListener(Quests_OnQuestDone);
        quests.OnAllQuestsDone.AddListener(Quests_OnAllQuestsDone);
        quests.TransitionQuest.OnDone.AddListener(Quests_OnTransitionQuestDone);
        quests.Activate();
    }

    private void Deactivate(Quests quests)
    {
        quests.OnQuestStart.RemoveListener(Quests_OnQuestStart);
        quests.OnQuestDone.RemoveListener(Quests_OnQuestDone);
        quests.OnAllQuestsDone.RemoveListener(Quests_OnAllQuestsDone);
        quests.TransitionQuest.OnDone.RemoveListener(Quests_OnTransitionQuestDone);
        quests.Deactivate();
    }

    private void GameManager_OnDayNightSwitch()
    {
        Deactivate(Current);

        currentDayNumber = GameManager.Instance.DayNumber;
        isDay = GameManager.Instance.IsDay;

        if ((currentDayNumber, isDay) == lastTimeBlock)
        {
            Debug.Log("Game Over.");
            return;
        }

        Activate(Current);
        OnQuestsInitialized.Invoke();
        Current.Start();
    }

    private void Quests_OnQuestStart(QuestEventArgs e)
    {
        OnQuestStart.Invoke(e);
    }

    private void Quests_OnQuestDone(QuestEventArgs e)
    {
        OnQuestDone.Invoke(e);
    }

    private void Quests_OnAllQuestsDone()
    {
        OnAllQuestsDone.Invoke();
    }

    private void Quests_OnTransitionQuestDone(QuestEventArgs e)
    {
        OnTransitionQuestDone.Invoke(e);
    }

    [Serializable]
    public class Quests
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
        /// Quest which will start when all quests of day or night are done. When this quest is completed transition to
        /// day or night will happen.
        /// </summary>
        [field: SerializeField]
        [Tooltip("Quest which will start when all quests of day or night are done. When this quest is completed transition to day or night will happen.")]
        public Quest TransitionQuest { get; private set; }

        /// <summary>
        /// Contains active quests.
        /// </summary>
        public IReadOnlyList<Quest> ActiveQuests => quests;

        /// <summary>
        /// Contains all quests from child quest queue and all active quests (in this order).
        /// </summary>
        public IEnumerable<Quest> AllQuests => ChildQuestQueue.Quests.Concat(ActiveQuests);

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
        /// Occur when all quest and quest queues are done. Transition quest will begin after this event.
        /// </summary>
        [field: SerializeField]
        [Tooltip("Occur when all quest and quest queues are done. Transition quest will begin after this event.")]
        public UnityEvent OnAllQuestsDone { get; private set; } = new();

        public void Activate()
        {
            GameManager.Instance.OnDayBegin.AddListener(GameManager_OnDayBegin);

            ChildQuestQueue.OnQuestStart.AddListener(QuestQueue_OnQuestStart);
            ChildQuestQueue.OnQuestDone.AddListener(QuestQueue_OnQuestDone);
            ChildQuestQueue.OnAllQuestsDone.AddListener(QuestQueue_OnAllQuestDone);
            foreach (var quest in quests)
            {
                quest.OnStart.AddListener(Quest_OnStart);
                quest.OnDone.AddListener(Quest_OnDone);
            }
            TransitionQuest.OnStart.AddListener(Quest_OnStart);
            TransitionQuest.OnDone.AddListener(TransitionQuest_OnDone);
            OnAllQuestsDone.AddListener(QuestManager_OnAllQuestsDone);
        }

        public void Deactivate()
        {
            GameManager.Instance.OnDayBegin.RemoveListener(GameManager_OnDayBegin);

            ChildQuestQueue.OnQuestStart.RemoveListener(QuestQueue_OnQuestStart);
            ChildQuestQueue.OnQuestDone.RemoveListener(QuestQueue_OnQuestDone);
            ChildQuestQueue.OnAllQuestsDone.RemoveListener(QuestQueue_OnAllQuestDone);
            foreach (var quest in quests)
            {
                quest.OnStart.RemoveListener(Quest_OnStart);
                quest.OnDone.RemoveListener(Quest_OnDone);
            }
            TransitionQuest.OnStart.RemoveListener(Quest_OnStart);
            TransitionQuest.OnDone.RemoveListener(TransitionQuest_OnDone);
            OnAllQuestsDone.RemoveListener(QuestManager_OnAllQuestsDone);
        }

        public void Start()
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

            questNotDoneCounter = 0;
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

        private void TransitionQuest_OnDone(QuestEventArgs e)
        {
            OnQuestDone.Invoke(new QuestEventArgs(e.Quest));
        }

        private void QuestManager_OnAllQuestsDone()
        {
            TransitionQuest.Start();
        }
    }
}