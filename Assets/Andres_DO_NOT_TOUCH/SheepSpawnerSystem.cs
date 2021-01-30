using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public class SheepSpawnerSystem : MonoBehaviour {
    private EntityManager entityManager;
    private EntityArchetype entityArchetype;

    private void Start() {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityArchetype = entityManager.CreateArchetype(typeof(Translation), typeof(Rotation), typeof(LocalToWorld), typeof(SheepRenderer));

        for (var i = 0; i < SheepScriptableRendererFeature.MAX_SHEEP; ++i) {
            CreateInstance(entityArchetype, i);
        }
    }

    private void CreateInstance(EntityArchetype archetype, int idx) {
        var entity = entityManager.CreateEntity(archetype);
#if UNITY_EDITOR
        entityManager.SetName(entity, "SheepRenderer" + idx);
#endif
        entityManager.AddComponentData(entity, new SheepRenderer() {dead = false});
        entityManager.AddComponentData(entity, new Translation {Value = new float3(UnityEngine.Random.value * 100f, 0.0f, UnityEngine.Random.value * 100f)});
        entityManager.AddComponentData(entity, new Rotation {Value = quaternion.identity});
        entityManager.SetEnabled(entity, true);
    }
}