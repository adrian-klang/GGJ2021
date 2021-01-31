using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPushRadius : MonoBehaviour {
    public GameConfig GameConfig;
    public SphereCollider sphereCollider;
    public Dog dog;

    private float radiusSqr;
    private List<Sheep> sheeps = new List<Sheep>();

    private void Update() {
        sphereCollider.radius = GameConfig.DogPushRadius;
        radiusSqr = GameConfig.DogPushRadius * GameConfig.DogPushRadius;
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
        foreach (var sheep in sheeps) {
            if (sheep == null) {
                continue;
            }
            
            var l = (transform.position - sheep.transform.position).sqrMagnitude / radiusSqr;
            var f = (sheep.transform.position - transform.position).normalized;
            sheep.Rigidbody.AddForce(f * (force * GameConfig.DogPushCurve.Evaluate(l)));
            dog.PlayScaryAudio();
        }
    }
}
