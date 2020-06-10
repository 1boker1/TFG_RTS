using JetBrains.Annotations;
using UnityEngine;

public abstract class Singleton<T> : Singleton where T : MonoBehaviour
{
    [CanBeNull] private static T _instance;

    [NotNull] private static readonly object Lock = new object();

    [SerializeField] private bool _persistent = true;

    [NotNull]
    public static T Instance
    {
        get
        {
            if (Quitting) return null;

            lock (Lock)
            {
                if (_instance != null) return _instance;

                var instances = FindObjectsOfType<T>();

                if (instances.Length > 0)
                {
                    if (instances.Length == 1) return _instance = instances[0];

                    for (var i = 1; i < instances.Length; i++) Destroy(instances[i]);

                    return _instance = instances[0];
                }

                return _instance = new GameObject($"({nameof(Singleton)}){typeof(T)}").AddComponent<T>();
            }
        }
    }

    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake() { }
}

public abstract class Singleton : MonoBehaviour
{
    public static bool Quitting { get; private set; }

    private void OnApplicationQuit() => Quitting = true;
}
