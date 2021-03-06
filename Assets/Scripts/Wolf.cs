﻿using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wolf : MonoBehaviour {
    public GameConfig Config;

    [HideInInspector]
    public List<GameObject> AttackRadiusSheep = new List<GameObject>();
    [HideInInspector]
    public bool IsScared;
    [HideInInspector]
    public Vector3 DogScarePosition;

    public Animator animator;

    private Rigidbody rigidbody;
    private AudioSource audioSource;

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayGrowlAudio()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        audioSource.PlayOneShot(Config.WolfGrowl[Random.Range(0, Config.WolfGrowl.Count - 1)]);
    }

    private void Update() {
        if (rigidbody.velocity.magnitude > 0.1f) {
            animator.SetFloat("Run", 10);
            transform.forward = rigidbody.velocity.normalized;
        } else {
            animator.SetFloat("Run", 0);
        }
    }

    private void FixedUpdate() {
        if (IsScared) {
            GoAwayFromDog();
        } else {
            if (AttackRadiusSheep.Count > 0) {
                GoToSeenSheepPosition();
            } else if (SheepManagerSystem.Sheeps.Count > 0) {
                GoToAvgSheepPosition();
            }
        }
    }

    private void GoAwayFromDog() {
        var forceToAdd = (transform.position - DogScarePosition).normalized * Config.WolfMoveForce;

        rigidbody.AddForce(forceToAdd);
    }

    private void GoToAvgSheepPosition() {
        var avgPosition = Vector3.zero;

        foreach (var sheep in SheepManagerSystem.Sheeps) {
            if (sheep == null || !sheep.GetComponent<Sheep>().Tamed) {
                continue;
            }

            avgPosition += sheep.transform.position;
        }

        avgPosition /= SheepManagerSystem.Sheeps.Count;

        var forceToAdd = (avgPosition - transform.position).normalized * Config.WolfMoveForce;

        rigidbody.AddForce(forceToAdd);
    }

    private void GoToSeenSheepPosition() {
        GameObject closestSheep = AttackRadiusSheep[0];
        var wolfPos = transform.position;
        float closestDistSqrd = Mathf.Infinity;

        foreach (var sheep in AttackRadiusSheep) {
            if (sheep == null) {
                continue;
            }
            
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

    private void OnCollisionEnter(Collision other) {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        if (other.gameObject.CompareTag("Sheep")) {
            var sheep = other.gameObject.GetComponent<Sheep>();
            if (sheep.Tamed) {
                sheep.gameObject.transform.position = Vector3.down * 10;
                AttackRadiusSheep.Remove(other.gameObject);
                //SheepManagerSystem.Sheeps.Remove(sheep);
                //Destroy(other.gameObject);
                //entityManager.DestroyEntity(sheep.sheepEntity);
                sheep.PlayDieAudio();
            }
        }
    }
}