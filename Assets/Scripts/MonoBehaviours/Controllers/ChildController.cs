using System;
using System.Collections.Generic;

using UnityEngine;

public class ChildController : Singleton<ChildController>, IInteractable
{
    [SerializeField]
    private GameObject childSprite;

    private PathFollow pathFollow;

    /// <summary>
    /// Spawn position of the child.
    /// </summary>
    private Vector3 spawnPosition;
    [SerializeField]
    private List<Quest> questQueue = new();
    /// <summary>
    /// Index of current quest within the quest queue.
    /// </summary>
    private int questIndex = 0;
    /// <summary>
    /// Determine if first quest within the quest queue was accepted. When new day starts player needs to interact with
    /// the child in ordr to receive the first quest.
    /// </summary>
    private bool questQueueStarted = false;

    private Quest CurrentQuest => questQueue[questIndex];
    private SubQuest CurrentSubQuest => CurrentQuest.CurrentSubQuest;

    /// <summary>
    /// Occur when all quests are completed.
    /// </summary>
    public event EventHandler OnAllQuestsDone;
    /// <summary>
    /// Occur when quest starts.
    /// </summary>
    public event EventHandler<QuestEventArgs> OnQuestStart;
    /// <summary>
    /// Occur when quest is completed.
    /// </summary>
    public event EventHandler<QuestEventArgs> OnQuestDone;
    /// <summary>
    /// Occur when sub quest starts.
    /// </summary>
    public event EventHandler<SubQuestEventArgs> OnSubQuestStart;
    /// <summary>
    /// Occur when sub quest is completed.
    /// </summary>
    public event EventHandler<SubQuestEventArgs> OnSubQuestDone;

    protected override void Awake()
    {
        base.Awake();

        spawnPosition = transform.position;

        pathFollow = GetComponent<PathFollow>();

        GameManager.Instance.OnDayBegin += GameManager_OnDayBegin;
        GameManager.Instance.OnNightBegin += GameManager_OnNightBegin;
        OnSubQuestStart += Child_OnSubQuestStart;
        OnSubQuestDone += Child_OnSubQuestDone;
    }

    private void OnEnable() => childSprite.SetActive(true);

    private void OnDisable() => childSprite.SetActive(false);

    public void Interact(PlayerController player)
    {
        if (!questQueueStarted)
        {
            questQueueStarted = true;
            CurrentSubQuest.QuestAdvancer.enabled = true;
            OnQuestStart?.Invoke(this, new QuestEventArgs(CurrentQuest));
            OnSubQuestStart?.Invoke(this, new SubQuestEventArgs(CurrentQuest, CurrentSubQuest));
            return;
        }

        if (CurrentQuest.AllSubQuestsFinished)
        {
            Debug.Assert(questQueueStarted);

            OnQuestDone?.Invoke(this, new QuestEventArgs(CurrentQuest));
            CurrentQuest.Reset();

            questIndex++;
            if (questIndex >= questQueue.Count)
            {
                questQueueStarted = false;
                questIndex = 0;
                OnAllQuestsDone?.Invoke(this, new EventArgs());
                return;
            }

            CurrentSubQuest.QuestAdvancer.enabled = true;
            OnQuestStart?.Invoke(this, new QuestEventArgs(CurrentQuest));
            OnSubQuestStart?.Invoke(this, new SubQuestEventArgs(CurrentQuest, CurrentSubQuest));
            return;
        }
    }

    /// <summary>
    /// Finish current sub quest.
    /// </summary>
    public void FinishSubQuest()
    {
        Debug.Assert(!CurrentQuest.AllSubQuestsFinished && questQueueStarted);

        CurrentSubQuest.QuestAdvancer.enabled = false;
        OnSubQuestDone?.Invoke(this, new SubQuestEventArgs(CurrentQuest, CurrentSubQuest));

        CurrentQuest.FinishSubQuest();
        if (CurrentQuest.AllSubQuestsFinished)
        {
            Debug.Log("All sub quest finished.");
            return;
        }

        CurrentSubQuest.QuestAdvancer.enabled = true;
        OnSubQuestStart?.Invoke(this, new SubQuestEventArgs(CurrentQuest, CurrentSubQuest));
    }

    private void GameManager_OnDayBegin(object sender, EventArgs e)
    {
        transform.position = spawnPosition;
        enabled = true;
    }

    private void GameManager_OnNightBegin(object sender, EventArgs e)
    {
        enabled = false;
    }

    private void Child_OnSubQuestStart(object sender, SubQuestEventArgs e)
    {
        Debug.Log($"New sub quest: \"{e.SubQuest.Description}\"");

        if (e.SubQuest.ChildPosition == null)
            return;

        if (e.SubQuest.Teleport)
            transform.position = e.SubQuest.ChildPosition.localPosition;
        else
            pathFollow.Target = e.SubQuest.ChildPosition;
    }

    private void Child_OnSubQuestDone(object sender, SubQuestEventArgs e)
    {
        pathFollow.Target = null;
    }
}

/// <summary>
/// Quest related event arguments.
/// </summary>
/// <param name="Quest">Related quest.</param>
public record QuestEventArgs(Quest Quest);

/// <summary>
/// Sub quest related event arguments.
/// </summary>
/// <param name="Quest">Related quest.</param>
/// <param name="SubQuest">Related sub quest.</param>
public record SubQuestEventArgs(Quest Quest, SubQuest SubQuest);