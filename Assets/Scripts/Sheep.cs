using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manager per-agent
/// </summary>
public class Sheep : MonoBehaviour
{
    public Rigidbody2D Rigidbody;
    public CircleCollider2D inner, mid, outer;
    
    [Header("Force Multiplier")]
    public float InnerForceMultiplier;
    public float MidForceMultiplier;
    public float OuterForceMultiplier;
    
    [Header("Radius Size")]
    public float InnerRadius = 1f;
    public float MidRadius = 2f;
    public float OuterRadius = 3f;
    
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

        inner.radius = InnerRadius;
        mid.radius = MidRadius;
        outer.radius = OuterRadius;
    }

    private void AddForceInner()
    {
        Vector3 avgPosition = Vector3.zero;

        foreach (var sheep in InnerCollisions)
        {
            avgPosition += sheep.transform.position;
        }

        avgPosition /= InnerCollisions.Count;

        var forceToApply = (transform.position - avgPosition).normalized * InnerForceMultiplier;
        
        Rigidbody.AddForce(forceToApply);
    }
    
    private void AddForceMid()
    {
        Vector2 avgVelocity = Vector2.zero;

        foreach (var sheep in InnerCollisions)
        {
            // todo: remove getcomponent
            var rigidbody = sheep.GetComponentInParent<Rigidbody2D>();
            avgVelocity += rigidbody.velocity;
        }

        avgVelocity /= MidCollisions.Count;

        var forceToApply = avgVelocity * MidForceMultiplier;
        
        Rigidbody.AddForce(forceToApply);
    }
    
    private void AddForceOuter()
    {
        Vector3 avgPosition = Vector3.zero;

        foreach (var sheep in InnerCollisions)
        {
            avgPosition += sheep.transform.position;
        }

        avgPosition /= OuterCollisions.Count;

        var forceToApply = (transform.position + avgPosition).normalized * OuterForceMultiplier;

        Rigidbody.AddForce(forceToApply);
    }
}
