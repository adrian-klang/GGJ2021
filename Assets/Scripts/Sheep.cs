using System.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// manager per-agent
/// </summary>
public class Sheep : MonoBehaviour {
    public GameConfig Config;
    public Rigidbody Rigidbody;
    public Entity sheepEntity;

    private AudioSource audioSource;

    // Is this sheep owned by the player
    private bool tamed;
    public bool Tamed {
        get { return tamed; }
        set {
            if (tamed != value) {
                tamed = value;
                OnSetTamed(value);
            }
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayBaaAudio());
    }

    private IEnumerator PlayBaaAudio()
    {
        yield return new WaitForSeconds(Random.Range(0, 3));

        var randomChance = Random.Range(0f, 1f);
        if (randomChance < Config.SheepBaaChance)
        {
            audioSource.PlayOneShot(Config.SheepBaa[Random.Range(0, Config.SheepBaa.Count - 1)]);
        }

        yield return PlayBaaAudio();
    }

    private void OnSetTamed(bool tamed) {
        if (tamed) {
            PlayerWallet.Instance.AddCoins(Config.TamedSheepCoins);
        }
    }

    private void OnEnable() {
        SheepManagerSystem.Sheeps.Add(this);
    }

    private void OnDisable() {
        //SheepManagerSystem.Sheeps.Remove(this);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos() {
        var col = Gizmos.color;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Config.AlignmentRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Config.CohesionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Config.SeparationRadius);

        Gizmos.color = col;
    }
#endif
    
}
