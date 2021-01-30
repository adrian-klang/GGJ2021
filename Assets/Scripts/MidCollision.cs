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
        if (other.CompareTag("Sheep"))
        {
            sheep.MidCollisions.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            sheep.MidCollisions.Remove(other.gameObject);
        }
    }
}
