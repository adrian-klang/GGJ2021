using System.Collections;
using UnityEngine;

public class WolfAttackRadius : MonoBehaviour
{
    private Wolf wolf;
    private GameConfig Config;
    private SphereCollider collider;

    private void Start()
    {
        wolf = GetComponentInParent<Wolf>();
        Config = wolf.Config;
        collider = GetComponent<SphereCollider>();
    }
    
    private void Update()
    {
        collider.radius = Config.WolfAttackRadius;
    }
    
    private void OnTriggerEnter(Collider other)
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

            var dog = other.gameObject.GetComponent<Dog>();
            dog.PlayScaryAudio();
        }
    }
    
    private void OnTriggerExit(Collider other)
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
