using UnityEngine;
using System.Threading.Tasks;

public class AppricationController : MonoBehaviour
{
    [SerializeField] private ClientSingletron clientPrefab;
    [SerializeField] private HostSingletron hostPrefab;
    async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            HostSingletron hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();

            ClientSingletron clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();

            if (authenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }


}
