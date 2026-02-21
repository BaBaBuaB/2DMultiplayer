using UnityEngine;
using System.Threading.Tasks;

public class ClientSingletron : MonoBehaviour
{
    private static ClientSingletron instance;
    public ClientManager GameManager { get; private set; }
    public static ClientSingletron Instance
    {
        get
        {
            if(instance != null) { return instance; }
            instance = FindFirstObjectByType<ClientSingletron>();

            if(instance == null)
            {
                Debug.LogError("No ClientSingleton in the scene!");
                return null;
            }
            return instance;    
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);  
    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientManager();  

        return await GameManager.InitAsync();
    }
    
    private void OnDestroy()
    {
        GameManager?.Dispose();
    }

}
