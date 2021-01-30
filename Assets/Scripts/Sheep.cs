using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manager per-agent
/// </summary>
public class Sheep : MonoBehaviour {
    public GameConfig Config;
    public Rigidbody2D Rigidbody;

    private void OnEnable() {
        SheepManager.RegisterSheep(this);
    }

    private void OnDisable() {
        SheepManager.DeregisterSheep(this);
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
