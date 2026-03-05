using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderBoardDisplay leaderboardEntityPrefab;
    [SerializeField] private int entitiesToDisplay = 8;

    private NetworkList<LeaderBoardEntityState> leaderboardEntities;
    private List<LeaderBoardDisplay> entityDisplays = new List<LeaderBoardDisplay>();

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderBoardEntityState>();
    }

    private void HandleCoinsChanged(ulong clientId,int newCoins)
    {
        for(int i = 0; i<leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientId != clientId) { continue; }

            leaderboardEntities[i] = new LeaderBoardEntityState
            {
                ClientId = leaderboardEntities[i].ClientId,
                PlayerName = leaderboardEntities[i].PlayerName,
                Coins = newCoins
            };

            return;
        }
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        leaderboardEntities.Add(new LeaderBoardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });

        player.Wallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) =>
            HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        if(NetworkManager.ShutdownInProgress) { return; }

        foreach(LeaderBoardEntityState entity in leaderboardEntities)
        {
            if(entity.ClientId != player.OwnerClientId) { continue; }
            leaderboardEntities.Remove(entity);
            break;
        }

        player.Wallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) =>
            HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;

            foreach(LeaderBoardEntityState entity in leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderBoardEntityState>
                {
                    Type = NetworkListEvent<LeaderBoardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer)
        {
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
            foreach(TankPlayer player in players)
            {
                HandlePlayerSpawned(player);
            }
            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
        }

        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderBoardEntityState> changeEvent)
    {
        if(!gameObject.scene.isLoaded) { return; }

        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Add:
                if(!entityDisplays.Any(x=>x.CliendId == changeEvent.Value.ClientId))
                {
                    LeaderBoardDisplay leaderboardEntity =
                    Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                    leaderboardEntity.Initialise(
                        changeEvent.Value.ClientId,
                        changeEvent.Value.PlayerName,
                        changeEvent.Value.Coins);
                    entityDisplays.Add(leaderboardEntity);
                }                
                break;

            case NetworkListEvent<LeaderBoardEntityState>.EventType.Remove:
                LeaderBoardDisplay displayToRemove =
                    entityDisplays.FirstOrDefault(x => x.CliendId == changeEvent.Value.ClientId);
                if(displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }
                break;

            case NetworkListEvent<LeaderBoardEntityState>.EventType.Value:
                LeaderBoardDisplay displayToUpdate =
                    entityDisplays.FirstOrDefault(x => x.CliendId == changeEvent.Value.ClientId);
                if(displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }

        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

        for(int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();
            entityDisplays[i].gameObject.SetActive(i <= entitiesToDisplay - 1);
        }

        LeaderBoardDisplay myDisplay =
            entityDisplays.FirstOrDefault(x => x.CliendId == NetworkManager.Singleton.LocalClientId);

        if(myDisplay != null)
        {
            if(myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                leaderboardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
        }
    }    
}
