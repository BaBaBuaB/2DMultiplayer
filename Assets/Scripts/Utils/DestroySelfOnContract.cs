using System;
using UnityEngine;

public class DestroySelfOnContract : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);
    }
}
