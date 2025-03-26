using System.Collections.Generic;

using UnityEngine;

public class ChildController : MonoBehaviour, IInteractable
{
    public ChildItemScriptableObject RequestedItem => itemQueue[queueIndex];

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
            queueIndex = 0;
    }

    private void Awake()
    {
        Debug.Log($"Child requests new item \"{RequestedItem.Name}\".");
    }
}
