using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    public async void StartHost()
    {
        await HostSingletron.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingletron.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
}
