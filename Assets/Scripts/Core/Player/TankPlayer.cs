using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")] [SerializeField]
    private CinemachineCamera cinemachineCamera;

    [Header("Settings")] [SerializeField] private int ownerPriority = 15;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            cinemachineCamera.Priority = ownerPriority;
        }

    }
}
