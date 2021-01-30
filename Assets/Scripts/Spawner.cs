using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject sheep;
    public GameConfig Config;
    
    void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1f);
        
        for (int i = 0; i < Config.AgentsAmount; i++)
        {
            var position = new Vector2(Random.Range(-Config.SpawnRadius * 0.5f, Config.SpawnRadius * 0.5f), Random.Range(-Config.SpawnRadius * 0.5f, Config.SpawnRadius * 0.5f));
            Instantiate(sheep, position, Quaternion.identity);
        }
    }
}
