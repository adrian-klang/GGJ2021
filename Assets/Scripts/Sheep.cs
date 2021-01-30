using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manager per-agent
/// </summary>
public class Sheep : MonoBehaviour
{
    public Rigidbody2D Rigidbody;

    private void OnEnable() {
        SheepManager.RegisterSheep(this);
    }

    private void OnDisable() {
        SheepManager.DeregisterSheep(this);
    }
}
