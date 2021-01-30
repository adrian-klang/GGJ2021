using System.Collections;
using UnityEngine;

public class WolfAttackRadius : MonoBehaviour
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
        collider.radius = Config.WolfAttackRadius;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            var sheep = other.GetComponent<Sheep>();
            if (sheep.Tamed)
            {
                wolf.AttackRadiusSheep.Add(other.gameObject);
            }
        }

        if (other.CompareTag("Dog"))
        {
            wolf.IsScared = true;
            wolf.DogScarePosition = other.gameObject.transform.position;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            var sheep = other.GetComponent<Sheep>();
            if (sheep.Tamed)
            {
                wolf.AttackRadiusSheep.Remove(other.gameObject);
            }
        }
        
        if (other.CompareTag("Dog"))
        {
            StartCoroutine(GetUnscared());
        }
    }

    private IEnumerator GetUnscared()
    {
        yield return new WaitForSeconds(Config.WolfSecondsBeforeGettingUnscared);

        wolf.IsScared = false;
    }
}
