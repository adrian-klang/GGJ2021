﻿using UnityEngine;

/// <summary>
/// Attract to agent
/// </summary>
public class OuterCollision : MonoBehaviour
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
