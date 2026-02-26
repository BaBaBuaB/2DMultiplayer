using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SelectionButton
{
    public Button teamButton;
    public GameObject selectionBox;
    public Color color;
}
public class TeamSelector : MonoBehaviour
{
    [SerializeField] private TeamColorLookUp colorLookup;
    [SerializeField] private SpriteRenderer[] playerSprites;
    [SerializeField] private SelectionButton[] selectionButtons;
    [SerializeField] private int teamIndex = 0;

    public const string PlayerTeamKey = "PlayerTeamIndex";

    private void OnValidate()
    {
        for(int i = 0; i < selectionButtons.Length; i++)
        {
            selectionButtons[i].color = (Color)colorLookup.GetTeamColor(i);
        }   

        foreach(SelectionButton selection in selectionButtons)
        {
            selection.teamButton.image.color = selection.color;  
        }
    }
    private void Start()
    {
        teamIndex = PlayerPrefs.GetInt(PlayerTeamKey, 0);
        HandleTeamChanged();
    }

    public void HandleTeamChanged()
    {
        foreach (SelectionButton selection in selectionButtons)
        {
            selection.selectionBox.SetActive(false);            
        }

        foreach (SpriteRenderer sprite in playerSprites)
        {
            sprite.color = selectionButtons[teamIndex].color;
        }
        selectionButtons[teamIndex].selectionBox.SetActive(true);
    }

    public void SelectTeam(int teamIndex)
    {
        this.teamIndex = teamIndex;
        HandleTeamChanged();
    }

    public void SaveTeam()
    {
        PlayerPrefs.SetInt(PlayerTeamKey, teamIndex);
    }
}
