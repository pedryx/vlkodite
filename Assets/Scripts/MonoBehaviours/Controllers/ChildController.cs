using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Interactable))]
public class ChildController : Singleton<ChildController>
{
    [SerializeField]
    private GameObject childSprite;

    private PathFollow pathFollow;
    private Interactable interactable;

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
    [Tooltip("Occur when all quests are completed.")]
    public UnityEvent OnAllQuestsDone = new();
    /// <summary>
    /// Occur when quest starts.
    /// </summary>
    [Tooltip("Occur when quest starts.")]
    public UnityEvent<QuestEventArgs> OnQuestStart = new();
    /// <summary>
    /// Occur when quest is completed.
    /// </summary>
    [Tooltip("Occur when quest is completed.")]
    public UnityEvent<QuestEventArgs> OnQuestDone = new();
    /// <summary>
    /// Occur when sub quest starts.
    /// </summary>
    [Tooltip("Occur when sub quest starts.")]
    public UnityEvent<SubQuestEventArgs> OnSubQuestStart = new();
    /// <summary>
    /// Occur when sub quest is completed.
    /// </summary>
    [Tooltip("Occur when sub quest is completed.")]
    public UnityEvent<SubQuestEventArgs> OnSubQuestDone = new();

    protected override void Awake()
    {
        base.Awake();

        spawnPosition = transform.position;
        pathFollow = GetComponent<PathFollow>();

        GameManager.Instance.OnDayBegin.AddListener(GameManager_OnDayBegin);
        GameManager.Instance.OnNightBegin.AddListener(GameManager_OnNightBegin);
        OnSubQuestStart.AddListener(Child_OnSubQuestStart);
        OnSubQuestDone.AddListener(Child_OnSubQuestDone);

        interactable = GetComponent<Interactable>();
        interactable.OnInteract.AddListener(Interactable_OnInteract);
    }

    private void OnEnable() => childSprite.SetActive(true);

    private void OnDisable() => childSprite.SetActive(false);

    /// <summary>
    /// Finish current sub quest.
    /// </summary>
    public void FinishSubQuest()
    {
        Debug.Assert(!CurrentQuest.AllSubQuestsFinished && questQueueStarted);

        OnSubQuestDone.Invoke(new SubQuestEventArgs(CurrentQuest, CurrentSubQuest));

        CurrentQuest.FinishSubQuest();
        if (CurrentQuest.AllSubQuestsFinished)
        {
            interactable.InteractionEnabled = true;
            Debug.Log("All sub quest finished.");
            return;
        }

        OnSubQuestStart.Invoke(new SubQuestEventArgs(CurrentQuest, CurrentSubQuest));
    }

    public void Interactable_OnInteract(Interactable interactable)
    {
        interactable.InteractionEnabled = false;

        if (!questQueueStarted)
        {
            questQueueStarted = true;
            OnQuestStart.Invoke(new QuestEventArgs(CurrentQuest));
            OnSubQuestStart.Invoke(new SubQuestEventArgs(CurrentQuest, CurrentSubQuest));
            return;
        }

        if (CurrentQuest.AllSubQuestsFinished)
        {
            Debug.Assert(questQueueStarted);

            OnQuestDone.Invoke(new QuestEventArgs(CurrentQuest));
            CurrentQuest.Reset();

            questIndex++;
            if (questIndex >= questQueue.Count)
            {
                questQueueStarted = false;
                questIndex = 0;
                OnAllQuestsDone.Invoke();
                return;
            }

            OnQuestStart.Invoke(new QuestEventArgs(CurrentQuest));
            OnSubQuestStart.Invoke(new SubQuestEventArgs(CurrentQuest, CurrentSubQuest));
            return;
        }
    }

    private void GameManager_OnDayBegin()
    {
        transform.position = spawnPosition;
        enabled = true;
        interactable.InteractionEnabled = true;
    }

    private void GameManager_OnNightBegin()
    {
        enabled = false;
    }

    private void Child_OnSubQuestStart(SubQuestEventArgs e)
    {
        Debug.Log($"New sub quest: \"{e.SubQuest.Description}\"");

        if (e.SubQuest.ChildPosition == null)
            return;

        if (e.SubQuest.Teleport)
            transform.position = e.SubQuest.ChildPosition.localPosition;
        else
            pathFollow.Target = e.SubQuest.ChildPosition;
    }

    private void Child_OnSubQuestDone(SubQuestEventArgs e)
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