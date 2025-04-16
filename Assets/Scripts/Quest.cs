using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Quest
{
    [SerializeField]
    private List<SubQuest> subQuestsQueue;

    /// <summary>
    /// Queue of sub quests within this quest.
    /// </summary>
    public IReadOnlyList<SubQuest> SubQuestsQueue => subQuestsQueue;

    /// <summary>
    /// Index of current sub quest within the sub quests queue.
    /// </summary>
    public int CurrentSubQuestIndex { get; private set; } = 0;

    /// <summary>
    /// Queue of sub quests within this quest.
    /// </summary>
    public SubQuest CurrentSubQuest => subQuestsQueue[CurrentSubQuestIndex];

    /// <summary>
    /// Determine if all subquests are finished.
    /// </summary>
    public bool AllSubQuestsFinished { get; private set; } = false;

    /// <summary>
    /// Finish current sub quest.
    /// </summary>
    /// <returns>True if all sub quests are finished, otherwise false.</returns>
    public bool FinishSubQuest()
    {
        Debug.Assert(!AllSubQuestsFinished);

        CurrentSubQuestIndex++;
        if (CurrentSubQuestIndex == subQuestsQueue.Count)
        {
            AllSubQuestsFinished = true;
            CurrentSubQuestIndex = -1;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        CurrentSubQuestIndex = 0;
        AllSubQuestsFinished = false;
    }
}
