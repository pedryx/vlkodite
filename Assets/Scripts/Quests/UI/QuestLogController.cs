using System.Collections.Generic;

using UnityEngine;

public class QuestLogController : MonoBehaviour
{
    private readonly List<GameObject> questPanels = new();

    private int questPanelCount = 0;
    [SerializeField]
    private GameObject questPanelPrefab;

    private void Awake()
    {
        gameObject.SetActive(false);
        CreateQuestPanels();
        QuestManager.Instance.OnQuestsInitialized.AddListener(QuestManager_OnQuestsInitialized);
    }

    public void AppendCurrentQuests()
    {
        CreateQuestPanels();
    }

    private void CreateQuestPanels()
    {
        CreateQuestPanel(QuestManager.Instance.Current.ChildQuestQueue);
        foreach (var quest in QuestManager.Instance.Current.ActiveQuests)
        {
            CreateQuestPanel(quest);
        }
        CreateQuestPanel(QuestManager.Instance.Current.TransitionQuest);
    }

    private void CreateQuestPanel(Quest quest)
    {
        questPanelCount++;
        gameObject.SetActive(true);

        QuestPanelController questPanel = Instantiate(questPanelPrefab, transform)
            .GetComponent<QuestPanelController>();

        questPanel.Init(quest);
        questPanel.OnQuestFullyDone.AddListener(QuestPanel_OnQuestFullyDone);
        questPanels.Add(questPanel.gameObject);
    }

    private void CreateQuestPanel(QuestQueue questQueue)
    {
        questPanelCount++;
        gameObject.SetActive(true);

        QuestPanelController questPanel = Instantiate(questPanelPrefab, transform)
            .GetComponent<QuestPanelController>();

        questPanel.Init(questQueue);
        questPanel.OnQuestFullyDone.AddListener(QuestPanel_OnQuestFullyDone);
        questPanels.Add(questPanel.gameObject);
    }

    public void Restart()
    {
        foreach (var questPanel in questPanels)
            Destroy(questPanel);
        questPanels.Clear();
        questPanelCount = 0;
    }

    private void QuestPanel_OnQuestFullyDone(QuestPanelEventArgs e)
    {
        Destroy(e.QuestPanel.gameObject);

        questPanelCount--;
        if (questPanelCount == 0)
            gameObject.SetActive(false);
    }

    private void QuestManager_OnQuestsInitialized()
    {
        CreateQuestPanels();
    }
}
