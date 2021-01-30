using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPushRadius : MonoBehaviour {
    public GameConfig GameConfig;
    public CircleCollider2D circleCollider;
    
    private List<Sheep> sheeps = new List<Sheep>();

    private void Update() {
        circleCollider.radius = GameConfig.DogPushRadius;
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
            var f = sheep.transform.position - transform.position;
            f *= force;
            sheep.Rigidbody.AddForce(f);
        }
    }
}
