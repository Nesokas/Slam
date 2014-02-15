using UnityEngine;
using System.Collections;

public class MainComponentManager
{
    private static MainComponentManager _instance=null;
    private GameObject _main=null;

    public static void CreateInstance()
    {
        if (_instance == null) {
            _instance = new MainComponentManager();
            GameObject go = GameObject.Find("Main");
            if (go == null) {
                go = new GameObject("Main");
                // important: make game object persistent:
                Object.DontDestroyOnLoad(go);
            }
            _instance._main = go;
            // trigger instantiation of other singletons
            // Component c = MenuManager.Instance;
            // ...
        }
    }

    public static MainComponentManager Instance
    {
        get
        {
            if (_instance == null) {
                CreateInstance();
            }
            return _instance;
        }
    }

    public static T AddMainComponent<T>() where T : UnityEngine.Component
    {
        T t = Instance._main.GetComponent<T>();
        if (t != null) {
            return t;
        }
        return Instance._main.AddComponent<T>();
    }
}