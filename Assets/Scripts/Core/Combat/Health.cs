using System;
using UnityEngine;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    private bool isDead;
    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        if (IsServer)
        {
            ModifyHealth(-damageValue);
            return;
        }

        //TakeDamageServerRpc(damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        if (IsServer)
        {
            ModifyHealth(healValue);
            return;
        }

        //RestoreHealthServerRpc(healValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestoreHealthServerRpc(int healValue)
    {
        ModifyHealth(healValue);
    }

    private void ModifyHealth(int value)
    {
        if (isDead) { return; }

        Debug.Log("!");

        int newHealth = CurrentHealth.Value + value;
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        if (CurrentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            isDead = true;
        }
    }
}
