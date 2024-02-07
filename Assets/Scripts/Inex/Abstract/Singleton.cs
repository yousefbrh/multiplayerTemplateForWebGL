using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract singleton class
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private bool doNotDestroyOnLoad = false;
    
    private static T _instance;
    
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    // GameObject obj = new GameObject();
                    // _instance = obj.AddComponent<T>();
                    // throw error
                    Debug.LogError("No instance of " + typeof(T) + " found");
                }
            }
            return _instance;
        }
    }
    
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (doNotDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject); // destroy duplicate (this) object
        }
    }
}

