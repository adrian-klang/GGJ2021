using System;
using Unity.Entities;

[Serializable]
public struct SheepRenderer : IComponentData {
    public bool dead;
}