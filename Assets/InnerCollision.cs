using System;
using UnityEngine;

/// <summary>
/// Repel from agent
/// </summary>
public class InnerCollision : MonoBehaviour
{
    private Sheep sheep;

    private void Start()
    {
        sheep = GetComponentInParent<Sheep>();
    } 
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
    }
}
