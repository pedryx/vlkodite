using UnityEngine;

/// <summary>
/// Represent a type of item which could be requested by a child.
/// </summary>
[CreateAssetMenu(fileName = "ChildItemScriptableObject", menuName = "Scriptable Objects/ChildItemScriptableObject")]
public class ChildItemScriptableObject : ScriptableObject
{
    /// <summary>
    /// Name of the item.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Name of the item.")]
    public string Name { get; private set; }

    /// <summary>
    /// Color of the item. (This property is only temporary.)
    /// </summary>
    [field: SerializeField]
    [Tooltip("Color of the item. (This property is only temporary.)")]
    public Color Color { get; private set; } = Color.yellow;
}
