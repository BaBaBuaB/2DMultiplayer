using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;

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
        await HostSingletron.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingletron.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
}
