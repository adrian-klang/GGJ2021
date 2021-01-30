using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject sheep;
    public GameConfig Config;
    
    private EntityManager entityManager;
    private EntityArchetype entityArchetype;
    
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityArchetype = entityManager.CreateArchetype(typeof(Translation), typeof(Rotation), typeof(LocalToWorld), typeof(SheepRenderer));
        
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1f);
        
        for (int i = 0; i < Config.SheepAmount; i++)
        {
            var position = new Vector2(Random.Range(-Config.SpawnWorldWidth * 0.5f, Config.SpawnWorldWidth * 0.5f), Random.Range(-Config.SpawnWorldLength * 0.5f, Config.SpawnWorldLength * 0.5f));
            var prefab = Instantiate(sheep, position, Quaternion.identity);
            
            CreateInstance(entityArchetype, i, position, prefab);
        }
    }
    
    private void CreateInstance(EntityArchetype archetype, int idx, Vector2 position, GameObject prefab) {
        var entity = entityManager.CreateEntity(archetype);
#if UNITY_EDITOR
        entityManager.SetName(entity, "SheepRenderer" + idx);
#endif
        entityManager.AddComponentData(entity, new SheepRenderer() {dead = false});
        entityManager.AddComponentData(entity, new Translation {Value = new float3(position.x, position.y, 0.0f)});
        entityManager.AddComponentData(entity, new Rotation {Value = quaternion.identity});
        entityManager.SetEnabled(entity, true);
    }
}
