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
                instance = new();

            return instance;
        }
    }

    protected virtual void Awake()
    {
        instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }
}