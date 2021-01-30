using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPushRadius : MonoBehaviour
{
    private List<Sheep> sheeps = new List<Sheep>();

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Sheep")) {
            sheeps.Add(other.GetComponent<Sheep>());
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        
    }
}
