﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject Sheep;
    public GameObject Wolf;
    
    public GameConfig Config;
    public LevelConfig LevelConfig;
    public DayNightCycle DayNightCycle;
    public PlayerWallet PlayerWallet;
    
    private EntityManager entityManager;
    private EntityArchetype entityArchetype;

    private bool spawnedWolves;
    
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityArchetype = entityManager.CreateArchetype(typeof(Translation), typeof(Rotation), typeof(LocalToWorld), typeof(SheepRenderer));

        SpawnSheep();
    }

    void Update()
    {
        if (!spawnedWolves && DayNightCycle.IsItNight())
        {
            Debug.Log("It's night");
            SpawnWolves(DayNightCycle.DayCounter);
            DayNightCycle.DayCounter++;
            spawnedWolves = true;
        }
        else if (spawnedWolves && DayNightCycle.IsItDay())
        {
            DestroyAllWolves();
            Debug.Log("It's day");
            AddCoinsPerSheep();
            spawnedWolves = false;
        }
    }

    private void SpawnSheep()
    {
        // TODO: uncomment in final level when removing random spawn of sheep
        // var allSheep = SheepManager.Sheeps;
        // for (int i = 0; i < allSheep.Length; i++)
        // {
        //     var sheep = allSheep[i].gameObject;
        //     
        //     CreateInstance(entityArchetype, i, sheep.transform.position, sheep);
        // }
        
        for (int i = 0; i < Config.SheepAmount; i++)
        {
            var position = new Vector2(Random.Range(-Config.SpawnWorldWidth * 0.5f, Config.SpawnWorldWidth * 0.5f), Random.Range(-Config.SpawnWorldLength * 0.5f, Config.SpawnWorldLength * 0.5f));
            var prefab = Instantiate(Sheep, position, Quaternion.identity);
            
            prefab.GetComponent<Sheep>().sheepEntity = CreateInstance(entityArchetype, i, position, prefab);
        }
    }

    private void SpawnWolves(int dayCounter)
    {
        if (LevelConfig.WolvesPerDay == null)
        {
            return;
        }
        
        for (int i = 0; i < LevelConfig.WolvesPerDay[dayCounter]; i++)
        {
            var spawnPos = LevelConfig.WolfSpawners[Random.Range(0, LevelConfig.WolfSpawners.Count - 1)];

            Instantiate(Wolf, spawnPos, Wolf.transform.rotation);
        }
    }

    private void DestroyAllWolves()
    {
        var wolves = GameObject.FindGameObjectsWithTag("Wolf");
        for (int i = wolves.Length - 1; i >= 0; i--)
        {
            Destroy(wolves[i]);
        }
    }
    
    private void AddCoinsPerSheep()
    {
        foreach (var sheep in SheepManagerSystem.Sheeps)
        {
            if (sheep.Tamed)
            {
                PlayerWallet.AddCoins(Config.AliveSheepCoins);
            }
        }
    }
    
    private Entity CreateInstance(EntityArchetype archetype, int idx, Vector2 position, GameObject prefab) {
        var entity = entityManager.CreateEntity(archetype);
#if UNITY_EDITOR
        entityManager.SetName(entity, "SheepRenderer" + idx);
#endif
        entityManager.AddComponentData(entity, new SheepRenderer() {dead = false});
        entityManager.AddComponentData(entity, new Translation {Value = new float3(position.x, position.y, 0.0f)});
        entityManager.AddComponentData(entity, new Rotation {Value = quaternion.identity});
        entityManager.SetEnabled(entity, true);

        return entity;
    }
}
