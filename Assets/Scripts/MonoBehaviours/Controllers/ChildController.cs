using System;
using System.Collections.Generic;

using UnityEngine;

public class ChildController : MonoBehaviour, IInteractable
{
    private Vector3 spawnPosition;

    /// <summary>
    /// Queue of items which will be one by one requested by the child.
    /// </summary>
    [SerializeField]
    [Tooltip("Queue of items which will be one by one requested by the child.")]
    [InspectorName("ItemQueue")]
    private List<ChildItemScriptableObject> itemQueue = new();

    /// <summary>
    /// Index of currently requested item form <see cref="itemQueue"/>.
    /// </summary>
    private int queueIndex = 0;

    /// <summary>
    /// Item requested by current child task.
    /// </summary>
    public ChildItemScriptableObject RequestedItem => itemQueue[queueIndex];

    public event EventHandler OnAllTasksDone;

    private void Awake()
    {
        GameManager.Instance.OnDayBegin += GameManager_OnDayBegin;
        GameManager.Instance.OnNightBegin += Instance_OnNightBegin;
        spawnPosition = transform.position;

        Debug.Log($"Child requests new item \"{RequestedItem.Name}\".");
    }

    public void Interact(PlayerController player)
    {
        if (player.Item != RequestedItem)
        {
            Debug.Log("Invalid item.");
            return;
        }

        player.Item = null;
        RequestNewItem();
        Debug.Log($"Item used on child. Child requests new item \"{RequestedItem.Name}\".");
    }

    private void RequestNewItem()
    {
        queueIndex++;
        if (queueIndex >= itemQueue.Count)
        {
            queueIndex = 0;
            OnAllTasksDone?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameManager_OnDayBegin(object sender, EventArgs e)
    {
        transform.position = spawnPosition;
        enabled = true;
    }

    private void Instance_OnNightBegin(object sender, EventArgs e)
    {
        enabled = false;
    }
}