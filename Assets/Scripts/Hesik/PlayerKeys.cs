using UnityEngine;
using System.Collections.Generic;

public class PlayerKeys : MonoBehaviour
{
    public static PlayerKeys Instance;

    private void Awake()
    {
        Instance = this;
    }

    private HashSet<string> keys = new HashSet<string>();

    public event System.Action<string> OnKeyCollected;

    public void AddKey(string keyID)
    {
        keys.Add(keyID);
        Debug.Log("Key collected: " + keyID);
        OnKeyCollected?.Invoke(keyID);
    }


    public bool HasKey(string keyID)
    {
        return keys.Contains(keyID);
    }
}
