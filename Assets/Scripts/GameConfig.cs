using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameConfig")]
public class GameConfig : ScriptableObject {
    [Header("Manager")]
    public int SheepAmount = 10;
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
    public float WolfAttackRadius = 3f;
    public float WolfMoveForce = 5f;
    public float WolfChangeTargetDiff = 10f;
    public float WolfSecondsBeforeGettingUnscared = 3f;

    [Header("Day Night Cycle")]
    public float DayNightCycleDurationInSeconds = 10;
    [Range(0, 1)]
    public float DayNightThreshold = 0.75f;

    [Header("Camera")]
    [Range(0, 1)]
    public float FollowDamp = 0.5f;
    public float Distance = 10;

    [Header("Shop")]
    public int TamedSheepCoins = 1;
    public int AliveSheepCoins = 1;
    public int StartingCoinsCount;
    public int FenceValue = 10;
    public int WolfTrapValue = 10;

    [Header("UI")]
    public float MessageTextTimeout = 3;
}