using UnityEngine;

public class QuestLogController : MonoBehaviour
{
    private int questPanelCount = 0;
    [SerializeField]
    private GameObject questPanelPrefab;

    private void Awake()
    {
        gameObject.SetActive(false);
        CreateQuestPanels();
        GameManager.Instance.OnDayBegin.AddListener(GameManager_OnDayBegin);
        //QuestManager.Instance.TransitionQuest.OnStart.AddListener(TransitionQuest_OnStart);
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
    }

    private void CreateQuestPanel(QuestQueue questQueue)
    {
        questPanelCount++;
        gameObject.SetActive(true);

        QuestPanelController questPanel = Instantiate(questPanelPrefab, transform)
            .GetComponent<QuestPanelController>();

        questPanel.Init(questQueue);
        questPanel.OnQuestFullyDone.AddListener(QuestPanel_OnQuestFullyDone);
    }

    private void QuestPanel_OnQuestFullyDone(QuestPanelEventArgs e)
    {
        Destroy(e.QuestPanel.gameObject);

        questPanelCount--;
        if (questPanelCount == 0)
            gameObject.SetActive(false);
    }

    private void GameManager_OnDayBegin()
    {
        CreateQuestPanels();
    }

    //private void TransitionQuest_OnStart(QuestEventArgs e)
    //{
    //    CreateQuestPanel(e.Quest);
    //}
}
