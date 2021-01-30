using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manager per-agent
/// </summary>
public class Sheep : MonoBehaviour {
    public Rigidbody2D Rigidbody;
    
    private List<GameObject> innerCollisions = new List<GameObject>();
    private List<GameObject> midCollisions = new List<GameObject>();
    private List<GameObject> outerCollisions = new List<GameObject>();
    
    private void FixedUpdate()
    {
        // mid -> add force that is avg of all velocities of inner circle
        // inner -> avg all positions -> opposite
        // outer -> avg all positions
        
        // multiplier for force
        // modify radius of collider at runtime
    }


    // add sheep
    // remove sheep
}
