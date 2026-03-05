using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
        private NetworkObject playerPrefab;
    
        public Action<string> OnClientLeft;
    
        private Dictionary<ulong,string> clientIdToAuth = new Dictionary<ulong,string>();
        private Dictionary<string,UserData> authIdToUserData = new Dictionary<string,UserData>();
    
        public NetworkServer(NetworkManager networkManager,NetworkObject playerPrefab)
        {
            this.networkManager = networkManager;
            this.playerPrefab = playerPrefab;
    
            networkManager.ConnectionApprovalCallback += ApprovalCheck;
            networkManager.OnServerStarted += OnNetworkReady;
        }
    
        private void OnNetworkReady()
        {
            networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }
    
        private void OnClientDisconnect(ulong clientId)
        {
            if(clientIdToAuth.TryGetValue(clientId,out string authId))
            {
                clientIdToAuth.Remove(clientId);
                authIdToUserData.Remove(authId);
                OnClientLeft?.Invoke(authId);
            }
        }
    
        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request, 
            NetworkManager.ConnectionApprovalResponse response)
        {
            string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            UserData userData = JsonUtility.FromJson<UserData>(payload);
    
            clientIdToAuth[request.ClientNetworkId] = userData.userAuthenId;
            authIdToUserData[userData.userAuthenId] = userData;
    
            _ = SpawnPlayerDelayed(request.ClientNetworkId);
    
            response.Approved = true;
            response.CreatePlayerObject = false;
        }
    
        private async Task SpawnPlayerDelayed(ulong clientId)
        {
            await Task.Delay(1000);
    
            NetworkObject playerInstance = GameObject.Instantiate(playerPrefab,
                SpawnPoints.GetRandomSpawnPos(), Quaternion.identity);
    
            playerInstance.SpawnAsPlayerObject(clientId);
        }
    
    
        public UserData GetUserDataByClientId(ulong clientId)
        {
            if(clientIdToAuth.TryGetValue(clientId,out string authId))
            {
                if(authIdToUserData.TryGetValue(authId,out UserData data))
                {
                    return data;
                }
                return null;
            }
            return null;
        }
    
        public void Dispose() 
        { 
            if(networkManager == null) { return; }
    
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            networkManager.OnServerStarted -= OnNetworkReady;
    
            if (networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }
}
