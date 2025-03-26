using UnityEngine;

/// <summary>
/// Adds player interaction capabilities.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Player interacted with the object.
    /// </summary>
    void Interact(PlayerController player);
}
