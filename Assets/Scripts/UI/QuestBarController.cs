using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestBarController : MonoBehaviour
{
    [SerializeField] private GameObject questEntryPrefab;
    [SerializeField] private Transform questListParent;
    // [SerializeField] private float fadeDuration = 0.5f;

    private List<QuestEntry> activeQuests = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddQuest("Clean up the kitchen");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (activeQuests.Count > 0)
            {
                CompleteQuest(activeQuests[0]); // Just for debug: complete first quest
            }
        }
    }

    public void AddQuest(string questText)
    {
        GameObject newEntry = Instantiate(questEntryPrefab, questListParent);
        QuestEntry entry = newEntry.GetComponent<QuestEntry>();
        entry.SetText(questText);
        activeQuests.Add(entry);
    }

    public void CompleteQuest(QuestEntry entry)
    {
        entry.Complete(() =>
        {
            activeQuests.Remove(entry);
            Destroy(entry.gameObject);
        });
    }
}
