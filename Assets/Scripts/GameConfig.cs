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
    
    [Header("Force Multiplier")]
    public float InnerForceMultiplier = 1f;
    public float MidForceMultiplier = 1f;
    public float OuterForceMultiplier = 1f;
    
    [Header("Radius Size")]
    public float InnerRadius = 1f;
    public float MidRadius = 2f;
    public float OuterRadius = 3f;
}
