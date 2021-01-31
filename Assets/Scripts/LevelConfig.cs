using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("Spawners")]
    public List<Vector3> WolfSpawners = new List<Vector3>();
    
    [Header("Days")]
    public List<int> WolvesPerDay = new List<int>();
}
