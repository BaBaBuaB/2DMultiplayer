using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private Toggle privateToggle;

    private void Start()
    {
        if (ClientSingletron.Instance == null)
        {
            return;
        }
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public async void StartHost()
    {
        await HostSingletron.Instance.GameManager.StartHostAsync(privateToggle.isOn);
    }

    public async void StartClient()
    {
        await ClientSingletron.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
}
