using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
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
                    GameObject singletonObject = new GameObject($"[SingletonMono] {typeof(T).Name}");
                    _instance = singletonObject.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}