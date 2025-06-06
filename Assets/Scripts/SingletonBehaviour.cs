using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Singleton instance not found");
            }
            return _instance;
        }
    }

    protected bool dontDestroyOnload;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            _instance = (T)this;
            Initialize();
            if (dontDestroyOnload)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    protected virtual void Initialize()
    {

    }
}