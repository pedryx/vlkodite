using System;

using UnityEngine;

/// <summary>
/// Represent a part of a quest. Each quest is composed of multiple sub quests. Each sub quest is "go and interact with
/// a certain object in a certain way".
/// </summary>
[Serializable]
public class SubQuest
{
    /// <summary>
    /// Description of the sub quest.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Description of the sub quest.")]
    public string Description { get; private set; }

    /// <summary>
    /// When player finished his interaction with this quest advancer, sub quest will be completed.
    /// </summary>
    [field: SerializeField]
    [Tooltip("When player finished his interaction with this quest advancer, sub quest will be completed.")]
    public QuestAdvancer QuestAdvancer { get; private set; }
}
