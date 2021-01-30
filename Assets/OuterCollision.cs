using UnityEngine;

/// <summary>
/// Attract to agent
/// </summary>
public class OuterCollision : MonoBehaviour
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
