using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject sheep;
    public int agentsAmount = 10;
    public float spawnRadius = 5f;
    
    void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1f);
        
        for (int i = 0; i < agentsAmount; i++)
        {
            var position = new Vector2(Random.Range(-spawnRadius * 0.5f, spawnRadius * 0.5f), Random.Range(-spawnRadius * 0.5f, spawnRadius * 0.5f));
            Instantiate(sheep, position, Quaternion.identity);
        }
    }
}
