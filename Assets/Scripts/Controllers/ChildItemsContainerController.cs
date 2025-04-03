using UnityEngine;

public class ChildItemsContainer : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Type of items contained in the container.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Type of items contained in the container.")]
    public ChildItemScriptableObject Item { get; private set; }

    public void Interact(PlayerController player)
    {
        player.Item = Item;
        Debug.Log($"Item \"{Item.name}\" picked up.");
    }
}
