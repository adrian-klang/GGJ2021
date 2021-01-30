using UnityEngine;

public class WolfFollowRadius : MonoBehaviour
{
    private Wolf wolf;

    private void Start()
    {
        wolf = GetComponentInParent<Wolf>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            wolf.FollowRadiusSheep.Add(other.gameObject);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            wolf.FollowRadiusSheep.Remove(other.gameObject);
        }
    }
}
