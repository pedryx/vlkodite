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
        ChildController.Instance.OnQuestStart += Child_OnQuestStart;
        ChildController.Instance.OnQuestDone += Child_OnQuestDone;
        gameObject.SetActive(false);
    }

    private void AddQuest(Quest quest)
    {
        this.gameObject.SetActive(true);
        GameObject gameObject = Instantiate(questPanelPrefab, transform);
        questItems.Add(quest, gameObject);

        var textMeshPro = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = quest.Description;

        var questItem = gameObject.GetComponent<QuestItemController>();
        foreach (var subQuest in quest.SubQuestsQueue)
            questItem.AddSubQuest(subQuest);
    }

    private void RemoveQuest(Quest quest)
    {
        Destroy(questItems[quest]);
        questItems.Remove(quest);

        if (!questItems.Any())
            gameObject.SetActive(false);
    }

    private void Child_OnQuestStart(object sender, QuestEventArgs e) => AddQuest(e.Quest);

    private void Child_OnQuestDone(object sender, QuestEventArgs e) => RemoveQuest(e.Quest);
}
