using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    [SerializeField] private TeamColorLookUp teamColorLookup;
    [SerializeField] private TankPlayer player;
    [SerializeField] private SpriteRenderer[] playerSprites;

    [SerializeField] private int colorIndex;

    private void Start()
    {
        HandleTeamChanged(0, player.TeamIndex.Value);
        player.TeamIndex.OnValueChanged += HandleTeamChanged;
    }


    public void HandleTeamChanged(int oldTeamIndex, int newTeamIndex)
    {
        Debug.Log($"Team Changed : {newTeamIndex}");
        if (teamColorLookup.GetTeamColor(newTeamIndex) == null)
        {
            return;
        }

        Color teamColor = (Color)teamColorLookup.GetTeamColor(newTeamIndex);

        foreach (SpriteRenderer sprite in playerSprites)
        {
            sprite.color = teamColor;
        }
    }

    private void OnDestroy()
    {
        player.TeamIndex.OnValueChanged -= HandleTeamChanged;
    }
}
