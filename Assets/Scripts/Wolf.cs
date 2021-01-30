using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public GameConfig Config;
    
    [HideInInspector]
    public List<GameObject> AllSheep = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> AttackRadiusSheep = new List<GameObject>();
    [HideInInspector]
    public bool IsScared;
    [HideInInspector]
    public Vector3 DogScarePosition;

    private Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        AllSheep = GameObject.FindGameObjectsWithTag("Sheep").ToList();
    }
    
    private void FixedUpdate()
    {
        if (IsScared)
        {
            GoAwayFromDog();
        }
        else
        {
            if (AttackRadiusSheep.Count > 0)
            {
                GoToSeenSheepPosition();
            }
            else if (AllSheep.Count > 0)
            {
                GoToAvgSheepPosition();
            }
        }
    }

    private void GoAwayFromDog()
    {
        var forceToAdd = (transform.position - DogScarePosition).normalized * Config.WolfMoveForce;
        
        rigidbody.AddForce(forceToAdd);
    }
    
    private void GoToAvgSheepPosition()
    {
        var avgPosition = Vector3.zero;

        foreach (var sheep in AllSheep)
        {
            if (sheep == null || !sheep.GetComponent<Sheep>().Tamed)
            {
                continue;
            }
            
            avgPosition += sheep.transform.position;
        }

        avgPosition /= AllSheep.Count;
        
        var forceToAdd = (avgPosition - transform.position).normalized * Config.WolfMoveForce;
        
        rigidbody.AddForce(forceToAdd);
    }

    private void GoToSeenSheepPosition()
    {
        GameObject closestSheep = AttackRadiusSheep[0];
        var wolfPos = transform.position;
        float closestDistSqrd = Mathf.Infinity;
        
        foreach (var sheep in AttackRadiusSheep)
        {
            var sheepPos = sheep.transform.position;
            var sqrdDist = wolfPos.x * sheepPos.x + wolfPos.y * sheepPos.y + wolfPos.z * sheepPos.z;
            
            if (sqrdDist < closestDistSqrd + Config.WolfChangeTargetDiff) // add offset to avoid changing target continuously
            {
                closestDistSqrd = sqrdDist;
                closestSheep = sheep;
            }
        }
        
        var forceToAdd = (closestSheep.transform.position - transform.position).normalized * Config.WolfMoveForce;
        
        rigidbody.AddForce(forceToAdd);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Sheep"))
        {
            if (other.gameObject.GetComponent<Sheep>().Tamed)
            {
                AllSheep.Remove(other.gameObject);
                Destroy(other.gameObject);
            }
        }
    }
}
