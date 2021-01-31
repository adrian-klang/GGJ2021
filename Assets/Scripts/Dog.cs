using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour {
    public GameConfig GameConfig;
    
    [Space]
    public Rigidbody Rigidbody;
    public DogPushRadius PushRadius;
    public DogPullRadius PullRadius;
        
    private bool pull = false;
    private bool push = false;
    private Vector3 moveDir = Vector3.zero;
    
    void Update() {
        moveDir = Vector3.zero;
        
        // Down
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            moveDir.z = -1;
        }
        
        // Up
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
            moveDir.z = 1;
        }
        
        // Left
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            moveDir.x = -1;
        }
        
        // Right
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            moveDir.x = 1;
        }
        
        // Pull
        if (Input.GetMouseButtonDown(0)) {
            pull = true;
        }
        
        if (Input.GetMouseButtonUp(0)) {
            pull = false;
        }
        
        // Push
        if (Input.GetMouseButtonDown(1)) {
            push = true;
        }

        if (Input.GetMouseButtonUp(1)) {
            push = false;
        }
    }

    private void FixedUpdate() {
        if (moveDir != Vector3.zero) {
            Rigidbody.AddForce(moveDir * GameConfig.DogMoveForce);
        }

        if (push) {
            PushRadius.Apply(GameConfig.DogPushForce);
        }

        if (pull) {
            PullRadius.Apply(GameConfig.DogPullForce);
        }
    }

    private void OnDrawGizmos() {
        var col = Gizmos.color;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GameConfig.DogPullRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, GameConfig.DogPushRadius);

        Gizmos.color = col;
    }
}