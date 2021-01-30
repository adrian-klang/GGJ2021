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
        if (other.CompareTag("Sheep"))
        {
            sheep.InnerCollisions.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            sheep.InnerCollisions.Remove(other.gameObject);
        }
    }
}
