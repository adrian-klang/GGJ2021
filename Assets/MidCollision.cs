using System;
using UnityEngine;

/// <summary>
/// Follow direction of agent
/// </summary>
public class MidCollision : MonoBehaviour
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
