using UnityEngine;

public class HostSingletron : MonoBehaviour
{
    private static HostSingletron instance;

    public HostManager GameManager { get; private set; }
    public static HostSingletron Instance
    {
        get
        {
            if (instance != null) { return instance; }
            instance = FindFirstObjectByType<HostSingletron>();

            if (instance == null)
            {
                Debug.LogError("No HostSingleton in the scene!");
                return null;
            }
            return instance;
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        GameManager = new HostManager();
    }
    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
