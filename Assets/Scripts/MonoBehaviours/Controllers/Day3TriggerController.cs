using UnityEngine;

[RequireComponent(typeof(PlayerTrigger))]
public class Day3TriggerController : MonoBehaviour
{
    private void Awake()
    {
        var playerTrigger = GetComponent<PlayerTrigger>();
        playerTrigger.OnEnter.AddListener(PlayerTrigger_OnPlayerEnter);
    }

    private void PlayerTrigger_OnPlayerEnter()
    {
        if (GameManager.Instance.DayNumber != 3)
            return;
        if (!GameManager.Instance.IsDay)
            return;

        QuestQueue queue = QuestManager.Instance.Current.ChildQuestQueue;

        if (queue.QuestIndex != 0)
            return;

        queue.ActiveQuest.Complete();
    }
}
