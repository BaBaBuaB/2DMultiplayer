using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")] [SerializeField]
    private CinemachineCamera cinemachineCamera;
    
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet Wallet { get; private set; }

    [Header("Settings")] [SerializeField] private int ownerPriority = 15;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            cinemachineCamera.Priority = ownerPriority;
        }

    }
}
