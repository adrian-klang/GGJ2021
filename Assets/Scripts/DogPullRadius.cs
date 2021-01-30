using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPullRadius : MonoBehaviour {
    public GameConfig GameConfig;
    public CircleCollider2D circleCollider;

    private float radiusSqr;
    private List<Sheep> sheeps = new List<Sheep>();
    
    private void Update() {
        circleCollider.radius = GameConfig.DogPullRadius;
        radiusSqr = GameConfig.DogPullRadius * GameConfig.DogPullRadius;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var sheep = other.GetComponent<Sheep>();
        if (sheep != null) {
            sheeps.Add(sheep);
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        var sheep = other.GetComponent<Sheep>();
        if (sheep != null) {
            sheeps.Remove(sheep);
        }
    }

    public void Apply(float force) {
        foreach (var sheep in sheeps) {
            var l = (transform.position - sheep.transform.position).sqrMagnitude / radiusSqr;
            var f = (transform.position - sheep.transform.position).normalized;
            sheep.Rigidbody.AddForce(f * (force * GameConfig.DogPullCurve.Evaluate(l)));
        }
    }
}
