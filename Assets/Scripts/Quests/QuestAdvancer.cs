using UnityEngine;

/// <summary>
/// Adds quest advancement capability to an game object. When player finishes his interaction with this game object,
/// sub quest will be completed.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class QuestAdvancer : MonoBehaviour
{
    private Interactable interactable;

    /// <summary>
    /// Associated quest. Will be null until the quest is started for the first time.
    /// </summary>
    private Quest quest;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.InteractionEnabled = false;
        interactable.OnInteract.AddListener(Interactable_OnInteract);

        QuestManager.Instance.OnQuestStart.AddListener(QuestManager_OnQuestStart);
        QuestManager.Instance.OnQuestDone.AddListener(QuestManager_OnQuestDone);
    }

    private void Interactable_OnInteract(Interactable interactable)
    {
        Debug.Assert(quest != null);

        quest.Complete();
    }

    private void QuestManager_OnQuestStart(QuestEventArgs e)
    {
        if (e.Quest.QuestAdvancer != this)
            return;

        quest = e.Quest;
        interactable.InteractionEnabled = true;
    }

    private void QuestManager_OnQuestDone(QuestEventArgs e)
    {
        if (e.Quest.QuestAdvancer != this)
            return;
        
        interactable.InteractionEnabled = false;

        if (e.Quest != quest)
            Debug.LogWarning("Quest on quest advancer differs from quest in event params.");
    }
}