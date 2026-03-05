using System;
using UnityEngine;

public class DestroySelfOnContract : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(projectile.TeamIndex != -1)
        {
            if (col.TryGetComponent<TankPlayer>(out TankPlayer player))
            {
                if (player.TeamIndex.Value == projectile.TeamIndex)
                {
                    return;
                }
            }
        }        

        Destroy(gameObject);
    }
}
