using UnityEngine;

public class WolfSearchRadius : MonoBehaviour
{
    private Wolf wolf;
    private GameConfig Config;
    private CircleCollider2D collider;

    private void Start()
    {
        wolf = GetComponentInParent<Wolf>();
        Config = wolf.Config;
        collider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        collider.radius = Config.WolfSearchRadius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            var sheep = other.GetComponent<Sheep>();
            if (sheep.Tamed)
            {
                wolf.SearchRadiusSheep.Add(other.gameObject);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            var sheep = other.GetComponent<Sheep>();
            if (sheep.Tamed)
            {
                wolf.SearchRadiusSheep.Remove(other.gameObject);
            }
        }
    }
}
