using UnityEngine;

public class WolfSearchRadius : MonoBehaviour
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
            wolf.SearchRadiusSheep.Add(other.gameObject);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            wolf.SearchRadiusSheep.Remove(other.gameObject);
        }
    }
}
