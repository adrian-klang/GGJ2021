using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manager per-agent
/// </summary>
public class Sheep : MonoBehaviour
{
    public Rigidbody2D Rigidbody;
    
    [HideInInspector]
    public List<GameObject> InnerCollisions = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> MidCollisions = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> OuterCollisions = new List<GameObject>();
    
    private void FixedUpdate()
    {
        AddForceInner();
        AddForceMid();
        AddForceOuter();

        // mid -> add force that is avg of all velocities of mid circle
        // inner -> avg all positions -> opposite
        // outer -> avg all positions

        // multiplier for force
        // modify radius of collider at runtime
    }

    private void AddForceInner()
    {
        Vector3 avgPosition = Vector3.zero;

        foreach (var sheep in InnerCollisions)
        {
            avgPosition += sheep.transform.position;
        }

        avgPosition /= InnerCollisions.Count;

        Rigidbody.AddForce(-avgPosition);
    }
    
    private void AddForceMid()
    {
        Vector2 avgVelocity = Vector2.zero;

        foreach (var sheep in InnerCollisions)
        {
            // todo: remove getcomponent
            var rigidbody = sheep.GetComponent<Rigidbody2D>();
            avgVelocity += rigidbody.velocity;
        }

        avgVelocity /= MidCollisions.Count;

        Rigidbody.AddForce(avgVelocity);
    }
    
    private void AddForceOuter()
    {
        Vector3 avgPosition = Vector3.zero;

        foreach (var sheep in InnerCollisions)
        {
            avgPosition += sheep.transform.position;
        }

        avgPosition /= OuterCollisions.Count;

        Rigidbody.AddForce(avgPosition);
    }
}
