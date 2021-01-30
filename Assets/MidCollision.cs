using System;
using UnityEngine;

/// <summary>
/// Follow direction of agent
/// </summary>
public class MidCollision : MonoBehaviour
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
