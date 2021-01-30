using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPullRadius : MonoBehaviour
{
    public float Force = 10;
    
    private List<Sheep> sheeps = new List<Sheep>();

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

    public void Apply() {
        foreach (var sheep in sheeps) {
            var f = transform.position - sheep.transform.position;
            f *= Force;
            sheep.Rigidbody.AddForce(f);
        }
    }
}
