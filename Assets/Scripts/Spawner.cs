using Unity.Entities;
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
    public UI UI;
    
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
            UI.ShowMessage("Night has arrived. Prepare for being eaten alive!");
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
        var allSheep = SheepManagerSystem.Sheeps;
        for (int i = 0; i < allSheep.Count; i++)
        {
            var sheep = allSheep[i].gameObject;
            
            CreateInstance(entityArchetype, i, sheep.transform.position, sheep);
        }
        
        for (int i = 0; i < Config.SheepAmount; i++)
        {
            var position = new Vector3(Random.Range(0, Config.SpawnWorldWidth), 0, Random.Range(0, Config.SpawnWorldLength));
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

            var wolfGO = Instantiate(Wolf, spawnPos, Wolf.transform.rotation);
            wolfGO.GetComponent<Wolf>().PlayGrowlAudio();
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
    
    private void AddCoinsPerSheep() {
        var sheepCount = 0;
        var coins = 0;
        foreach (var sheep in SheepManagerSystem.Sheeps)
        {
            if (sheep.Tamed) {
                sheepCount++;
                coins += Config.AliveSheepCoins;
            }
        }
        
        PlayerWallet.AddCoins(coins);
        
        UI.ShowMessage($"You've survived the night! {sheepCount} sheep are still alive, you won {coins} coins!");
    }
    
    private Entity CreateInstance(EntityArchetype archetype, int idx, Vector3 position, GameObject prefab) {
        var entity = entityManager.CreateEntity(archetype);
#if UNITY_EDITOR
        entityManager.SetName(entity, "SheepRenderer" + idx);
#endif
        entityManager.AddComponentData(entity, new SheepRenderer() {dead = false});
        entityManager.AddComponentData(entity, new Translation {Value = position});
        entityManager.AddComponentData(entity, new Rotation {Value = quaternion.identity});
        entityManager.SetEnabled(entity, true);

        return entity;
    }
}
