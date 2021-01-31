using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPullRadius : MonoBehaviour {
    public GameConfig GameConfig;
    public SphereCollider sphereCollider;
    public Dog dog;

    private float radiusSqr;
    private List<Sheep> sheeps = new List<Sheep>();
    
    private void Update() {
        sphereCollider.radius = GameConfig.DogPullRadius;
        radiusSqr = GameConfig.DogPullRadius * GameConfig.DogPullRadius;
    }

    private void OnTriggerEnter(Collider other) {
        var sheep = other.GetComponent<Sheep>();
        if (sheep != null) {
            sheeps.Add(sheep);
        }
    }

    public void OnTriggerExit(Collider other) {
        var sheep = other.GetComponent<Sheep>();
        if (sheep != null) {
            sheeps.Remove(sheep);
        }
    }

    public void Apply(float force) {
        dog.PlayFriendlyAudio();
        
        foreach (var sheep in sheeps) {
            if (sheep == null) {
                continue;
            }
            
            var l = (transform.position - sheep.transform.position).sqrMagnitude / radiusSqr;
            var f = (transform.position - sheep.transform.position).normalized;
            sheep.Rigidbody.AddForce(f * (force * GameConfig.DogPullCurve.Evaluate(l)));
            sheep.Tamed = true;
        }
    }
}
