/// <summary>
/// Adds player interaction capabilities.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Determines whether the interaction is continuous (i.e., it occurs every frame while the interaction key is held
    /// down), or instant (i.e., it occurs only once when the key is initially pressed).
    /// </summary>
    bool IsContinuous => false;

    /// <summary>
    /// Player interacted with the object.
    /// </summary>
    void Interact(PlayerController player);
}
