using System;
using UnityEngine;

/// <summary>
/// Repel from agent
/// </summary>
public class InnerCollision : MonoBehaviour
{
    private FlockingBehaviour flockingBehaviour;

    private void Start()
    {
        flockingBehaviour = GetComponentInParent<FlockingBehaviour>();
    } 
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
    }
}
