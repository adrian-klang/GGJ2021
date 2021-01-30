using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct SheepRenderer : IComponentData {
    public bool dead;
}