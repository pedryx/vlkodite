using System.Linq;

using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour, new()
{
    private static T instance;

    public static bool Persistent
    {
        get => true;
    }

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectsByType<T>(FindObjectsSortMode.None).First();

            return instance;
        }
    }

    protected virtual void Awake()
    {
        instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }
}