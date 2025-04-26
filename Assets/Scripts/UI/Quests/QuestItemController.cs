using DG.Tweening;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TMPro;

using UnityEngine;

public class QuestItemController : MonoBehaviour
{
    private readonly Queue<Action> actionQueue = new();

    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private GameObject subQuestPanelPrefab;

    private bool fadeInProgress = false;
    private bool crossInProgress = false;

    [field: SerializeField]
    public float Duration { get; private set; } = 3.0f;

    public Quest Quest { get; set; }

    private void Awake()
    {
        ChildController.Instance.OnSubQuestStart.AddListener(Child_OnSubQuestStart);
        ChildController.Instance.OnSubQuestDone.AddListener(Child_OnSubQuestDone);
    }

    public void SafeDestroy(Action onDestroy)
    {
        EnqueueAction(() =>
        {
            onDestroy.Invoke();
            Destroy(gameObject);
        });
    }

    private void EnqueueAction(Action action)
    {
        if (fadeInProgress || crossInProgress)
            actionQueue.Enqueue(action);
        else
            action.Invoke();
    }

    private void AddSubQuest(SubQuest subQuest)
    {
        textMeshPro.text = subQuest.Description;

        fadeInProgress = true;
        canvasGroup.DOFade(1.0f, Duration).OnComplete(() =>
        {
            fadeInProgress = false;
            if (!crossInProgress && actionQueue.Any())
                actionQueue.Dequeue().Invoke();
        });
    }

    private void RemoveSubQuest()
    {
        crossInProgress = true;
        StartCoroutine(DoCross(textMeshPro));

        fadeInProgress = true;
        canvasGroup.DOFade(0.0f, Duration).OnComplete(() =>
        {
            fadeInProgress = false;
            if (!crossInProgress && actionQueue.Any())
                actionQueue.Dequeue().Invoke();
        });
    }

    private IEnumerator DoCross(TextMeshProUGUI textMeshPro)
    {
        string originalText = string.Copy(textMeshPro.text);
        int length = CountLength(originalText);
        // When fade is really low, the cross in no longer visible, so we make the croos two times faster.
        float timePerCharacter = (Duration / 2.0f) / length;

        for (int i = 0; i <= length; i++)
        {
            textMeshPro.text = Cross(originalText, i);
            yield return new WaitForSeconds(timePerCharacter);
        }

        crossInProgress = false;
        if (!fadeInProgress && actionQueue.Any())
            actionQueue.Dequeue().Invoke();
    }

    private int CountLength(string str)
    {
        bool counting = true;
        int count = 0;

        foreach (char c in str)
        {
            if (c == '<')
            {
                counting = false;
                continue;
            }
            if (c == '>')
            {
                counting = true;
                continue;
            }

            if (counting)
                count++;
        }

        return count;
    }

    private string Cross(string str, int crossLength)
    {
        const string crossStart = "<s>";
        const string crossEnd = "</s>";

        bool ignore = false;
        StringBuilder builder = new();
        int currentCrossLength = 0;
        int crossEndIndex = 0;

        builder.Append(crossStart);


        for (int i = 0; i < str.Length; i++)
        {
            if (currentCrossLength == crossLength)
            {
                crossEndIndex = i;
                builder.Append(crossEnd);
                break;
            }

            char c = str[i];

            if (c == '<')
            {
                ignore = true;
                builder.Append(crossEnd);
            }

            builder.Append(c);

            if (c == '>')
            {
                ignore = false;
                builder.Append(crossStart);
            }

            if (!ignore)
                currentCrossLength++;
        }

        if (crossEndIndex == str.Length - 1)
            builder.Append(crossEnd);
        else
            builder.Append(str[crossEndIndex..]);

        return builder.ToString();
    }

    private void Child_OnSubQuestStart(SubQuestEventArgs e)
    {
        if (e.Quest != Quest)
            return;

        EnqueueAction(() => AddSubQuest(e.SubQuest));
    }

    private void Child_OnSubQuestDone(SubQuestEventArgs e)
    {
        if (e.Quest != Quest)
            return;

        EnqueueAction(() => RemoveSubQuest());
    }
}
