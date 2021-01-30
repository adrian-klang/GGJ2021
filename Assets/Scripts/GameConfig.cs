﻿using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config)")]
public class GameConfig : ScriptableObject {
    [Header("Manager")]
    public int AgentsAmount = 10;
    public float SpawnWorldWidth = 5f;
    public float SpawnWorldLength = 5f;
    
    [Header("Dog")]
    public float DogMoveForce = 10;
    public float DogPushForce = 10;
    public float DogPushRadius = 10;
    public AnimationCurve DogPushCurve;
    public float DogPullForce = 10;
    public float DogPullRadius = 10;
    public AnimationCurve DogPullCurve;
    
    [Header("Sheep")]
    public float SeparationForce = 1f;
    public float AlignmentForce = 1f;
    public float CohesionForce = 1f;
    public float SeparationRadius = 1f;
    public float AlignmentRadius = 2f;
    public float CohesionRadius = 3f;

    [Header("Wolf")]
    public float FollowRadius = 3f;
    public float SearchRadius = 10f;
    public float WolfForce = 5f;
    public float WolfChangeTargetDiff = 20f;

    [Header("Day Night Cycle")]
    public float DayNightCycleSpeed = 10;

    [Header("Camera")]
    [Range(0, 1)]
    public float FollowDamp = 0.5f;
    public float Distance = 10;
}
