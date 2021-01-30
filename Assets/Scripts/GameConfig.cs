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
}
