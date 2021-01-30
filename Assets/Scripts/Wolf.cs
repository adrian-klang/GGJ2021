using System;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public GameConfig Config;
    
    [HideInInspector]
    public List<GameObject> SearchRadiusSheep = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> FollowRadiusSheep = new List<GameObject>();

    private Rigidbody2D rigidbody;
    private bool isScared;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        if (isScared)
        {
            
        }
        else
        {
            if (FollowRadiusSheep.Count > 0)
            {
                GoToSeenSheepPosition();
            }
            else if (SearchRadiusSheep.Count > 0)
            {
                GoToAvgSheepPosition();
            }
        }
    }

    private void GoToAvgSheepPosition()
    {
        var avgPosition = Vector3.zero;
        
        foreach (var sheep in SearchRadiusSheep)
        {
            avgPosition += sheep.transform.position;
        }

        avgPosition /= SearchRadiusSheep.Count;
        
        var forceToAdd = (avgPosition - transform.position).normalized * Config.WolfForce;
        
        rigidbody.AddForce(forceToAdd);
    }

    private void GoToSeenSheepPosition()
    {
        GameObject closestSheep = FollowRadiusSheep[0];
        var wolfPos = transform.position;
        float closestDistSqrd = Mathf.Infinity;
        
        foreach (var sheep in FollowRadiusSheep)
        {
            var sheepPos = sheep.transform.position;
            var sqrdDist = wolfPos.x * sheepPos.x + wolfPos.y * sheepPos.y + wolfPos.z * sheepPos.z;
            
            if (sqrdDist < closestDistSqrd + Config.WolfChangeTargetDiff) // add offset to avoid changing target continuously
            {
                closestDistSqrd = sqrdDist;
                closestSheep = sheep;
            }
        }
        
        var forceToAdd = (closestSheep.transform.position - transform.position).normalized * Config.WolfForce;
        
        rigidbody.AddForce(forceToAdd);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Sheep"))
        {
            Destroy(other.gameObject);
        }
    }
}
