using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour {
    private static T _instance;
    public static T Instance {
        get {
            if (_instance == null) {
                if (Debug.isDebugBuild) {//开发版本Debug，发布版本不Debug，降低性能影响
                    Debug.LogError(typeof(T) + " has no instance");
                }
            }
            return _instance;
        }
    }

    private void Awake() {
        if (_instance != null) {
            if (Debug.isDebugBuild) {
                Debug.LogError(typeof(T) + " had has an instance");
            }
        }
        _instance = this as T;
        InitAwake();
    }

    protected virtual void InitAwake() { }//子类重写该方法当作Awake
}
