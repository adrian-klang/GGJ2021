using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manager per-agent
/// </summary>
public class Sheep : MonoBehaviour
{
    public GameConfig Config;
    public Rigidbody2D Rigidbody;
    public CircleCollider2D inner, mid, outer;
    
    [HideInInspector]
    public List<GameObject> InnerCollisions = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> MidCollisions = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> OuterCollisions = new List<GameObject>();

    private void FixedUpdate()
    {
        if (InnerCollisions.Count > 0)
        {
            AddForceInner();
        }

        if (MidCollisions.Count > 0)
        {
            AddForceMid();
        }

        if (OuterCollisions.Count > 0)
        {
            AddForceOuter();
        }
        
        inner.radius = Config.SeparationRadius;
        mid.radius = Config.AlignmentRadius;
        outer.radius = Config.CohesionRadius;
    }

    private void AddForceInner()
    {
        Vector3 avgPosition = Vector3.zero;

        foreach (var sheep in InnerCollisions)
        {
            avgPosition += sheep.transform.position;
        }

        avgPosition /= InnerCollisions.Count;
        
        var forceToApply = (transform.position - avgPosition).normalized * Config.SeparationForce;
        
        Rigidbody.AddForce(forceToApply);
    }
    
    private void AddForceMid()
    {
        Vector2 avgVelocity = Vector2.zero;

        foreach (var sheep in MidCollisions)
        {
            // todo: remove getcomponent
            var rigidbody = sheep.GetComponentInParent<Rigidbody2D>();
            avgVelocity += rigidbody.velocity;
        }

        avgVelocity /= MidCollisions.Count;

        var forceToApply = avgVelocity * Config.AlignmentForce;
        
        Rigidbody.AddForce(forceToApply);
    }
    
    private void AddForceOuter()
    {
        Vector3 avgPosition = Vector3.zero;

        foreach (var sheep in OuterCollisions)
        {
            avgPosition += sheep.transform.position;
        }

        avgPosition /= OuterCollisions.Count;

        var forceToApply = (avgPosition - transform.position).normalized * Config.CohesionForce;

        Rigidbody.AddForce(forceToApply);
    }
}
