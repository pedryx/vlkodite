using DG.Tweening;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

public class QuestPanelController : MonoBehaviour
{
    private readonly Queue<Action> actionQueue = new();

    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    [SerializeField]
    private CanvasGroup canvasGroup;

    private bool wasInit = false;
    private bool fadeInProgress = false;
    private bool crossInProgress = false;

    [field: SerializeField]
    public float Duration { get; private set; } = 3.0f;

    /// <summary>
    /// Occurs when quest is done AND ALL UI TWEENS ARE DONE.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Occurs when quest is done AND ALL UI TWEENS ARE DONE.")]
    public UnityEvent<QuestPanelEventArgs> OnQuestFullyDone { get; private set; } = new();

    public void Init(Quest quest)
    {
        Debug.Assert(!wasInit);
        wasInit = true;

        quest.OnStart.AddListener(Quest_OnStart);
        quest.OnDone.AddListener(Quest_OnDone);
        gameObject.SetActive(false);
    }

    public void Init(QuestQueue questQueue)
    {
        Debug.Assert(!wasInit);
        wasInit = true;

        questQueue.OnQuestStart.AddListener(QuestQueue_OnQuestStart);
        questQueue.OnQuestDone.AddListener(QuestQueue_OnQuestDone);
        questQueue.OnAllQuestsDone.AddListener(QuestQueue_OnAllQuestsDone);
        gameObject.SetActive(false);
    }

    private void SetQuest(Quest quest)
    {
        textMeshPro.text = quest.Description;
        if (quest.Description == "")
            Destroy(gameObject);

        fadeInProgress = true;
        canvasGroup.DOFade(1.0f, Duration).OnComplete(() =>
        {
            fadeInProgress = false;
            if (!crossInProgress && actionQueue.Any())
                actionQueue.Dequeue().Invoke();
        });
    }

    private void ClearQuest()
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

    private void EnqueueAction(Action action)
    {
        if (fadeInProgress || crossInProgress)
            actionQueue.Enqueue(action);
        else
            action.Invoke();
    }

    #region Text Cross Tween
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


        for (int i = 0; i <= str.Length; i++)
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
        {
            builder.Append(str.Last());
            builder.Append(crossEnd);
        }
        else
            builder.Append(str[crossEndIndex..]);

        return builder.ToString();
    }
    #endregion

    private void Quest_OnStart(QuestEventArgs e)
    {
        EnqueueAction(() => SetQuest(e.Quest));
        gameObject.SetActive(true);
    }

    private void Quest_OnDone(QuestEventArgs e)
    {
        EnqueueAction(() => ClearQuest());
        EnqueueAction(() => OnQuestFullyDone.Invoke(new QuestPanelEventArgs(this, e.Quest)));
    }

    private void QuestQueue_OnQuestStart(QuestQueueEventArgs e)
    {
        EnqueueAction(() => SetQuest(e.Quest));
        gameObject.SetActive(true);
    }

    private void QuestQueue_OnQuestDone(QuestQueueEventArgs e)
    {
        EnqueueAction(() => ClearQuest());
    }

    private void QuestQueue_OnAllQuestsDone(QuestQueueEventArgs e)
    {
        EnqueueAction(() => OnQuestFullyDone.Invoke(new QuestPanelEventArgs(this, e.Quest)));
    }
}

/// <summary>
/// Quest panel related events arguments.
/// </summary>
/// <param name="QuestPanel">Related quest panel.</param>
/// <param name="Quest">Related quest.</param>
public record QuestPanelEventArgs(QuestPanelController QuestPanel, Quest Quest);