using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config)")]
public class GameConfig : ScriptableObject {
    [Header("Dog")]
    public float DogMoveForce = 10;
    public float DogPushForce = 10;
    public float DogPushRadius = 10;
    public float DogPullForce = 10;
    public float DogPullRadius = 10;
    
    [Header("Sheep Forces")]
    public float SeparationForce = 1f;
    public float AlignmentForce = 1f;
    public float CohesionForce = 1f;
    
    [Header("Sheep Radii")]
    public float SeparationRadius = 1f;
    public float AlignmentRadius = 2f;
    public float CohesionRadius = 3f;
}
