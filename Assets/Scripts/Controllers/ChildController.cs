using System;
using System.Collections.Generic;

using UnityEngine;

// TODO: make disabled on night

public class ChildController : MonoBehaviour, IInteractable
{
    private Vector2 startPosition;

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

    public ChildItemScriptableObject RequestedItem => itemQueue[queueIndex];

    public event EventHandler OnAllTasksDone;

    public void Interact(PlayerController player)
    {
        if (GameManager.Instance.IsNight)
            return;

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

    private void Awake()
    {
        GameManager.Instance.OnDayBegin += GameManager_OnDayBegin;
        startPosition = transform.position;

        Debug.Log($"Child requests new item \"{RequestedItem.Name}\".");
    }

    private void GameManager_OnDayBegin(object sender, EventArgs e)
    {
        // TODO: change sprite
        GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        transform.position = startPosition;
    }
}