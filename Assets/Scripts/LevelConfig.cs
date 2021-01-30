using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("Spawners")]
    public List<Transform> WolfSpawners = new List<Transform>();
    
    [Header("Days")]
    public List<int> WolvesPerDay = new List<int>();
}
