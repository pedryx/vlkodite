using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;

public class QuestLogController : MonoBehaviour
{
    private readonly Dictionary<Quest, GameObject> questItems = new();

    [SerializeField]
    private GameObject questPanelPrefab;

    private void Awake()
    {
        ChildController.Instance.OnQuestStart.AddListener(Child_OnQuestStart);
        ChildController.Instance.OnQuestDone.AddListener(Child_OnQuestDone);
        gameObject.SetActive(false);
    }

    private void AddQuest(Quest quest)
    {
        this.gameObject.SetActive(true);
        GameObject gameObject = Instantiate(questPanelPrefab, transform);
        questItems.Add(quest, gameObject);

        var textMeshPro = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
            textMeshPro.text = quest.Description;

        var questItemController = gameObject.GetComponent<QuestItemController>();
        questItemController.Quest = quest;
    }

    private void RemoveQuest(Quest quest)
    {
        questItems[quest].GetComponent<QuestItemController>().SafeDestroy(() =>
        {
            questItems.Remove(quest);

            if (!questItems.Any())
                gameObject.SetActive(false);

        });
    }

    private void Child_OnQuestStart(QuestEventArgs e) => AddQuest(e.Quest);

    private void Child_OnQuestDone(QuestEventArgs e) => RemoveQuest(e.Quest);
}
