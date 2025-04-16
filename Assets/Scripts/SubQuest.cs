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

    /// <summary>
    /// New position of the child after sub quest starts.
    /// </summary>
    [field: SerializeField]
    [Tooltip("New position of the child after sub quest starts.")]
    public Transform ChildPosition { get; private set; } = null;

    /// <summary>
    /// Determine if child should teleport to this position, otherwise it will walk to it.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Determine if child should teleport to this position, otherwise it will walk to it.")]
    public bool Teleport { get; private set; } = false;
}
