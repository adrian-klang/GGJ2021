using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// manager per-agent
/// </summary>
public class Sheep : MonoBehaviour {
    public GameConfig Config;
    public Rigidbody2D Rigidbody;
    public Entity sheepEntity;

    // Is this sheep owned by the player
    private bool tamed;
    public bool Tamed {
        get { return tamed; }
        set {
            if (tamed != value) {
                tamed = value;
                OnSetTamed(value);
            }
        }
    }

    private void OnSetTamed(bool tamed) {
        if (tamed) {
            PlayerWallet.Instance.AddCoins(Config.TamedSheepCoins);
        }
    }

    private void OnEnable() {
        SheepManagerSystem.Sheeps.Add(this);
    }

    private void OnDisable() {
        SheepManagerSystem.Sheeps.Remove(this);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos() {
        var col = Gizmos.color;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Config.AlignmentRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Config.CohesionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Config.SeparationRadius);

        Gizmos.color = col;
    }
#endif
    
}
