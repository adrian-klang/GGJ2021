using UnityEngine;

public class Dog : MonoBehaviour {
    public GameConfig Config;
    public GameInput GameInput;
    
    [Space]
    public Rigidbody Rigidbody;
    public DogPushRadius PushRadius;
    public DogPullRadius PullRadius;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPushAudio()
    {
        var randomChance = Random.Range(0f, 1f);
        if (randomChance < Config.DogWoofScaryOnPushChance)
        {
            audioSource.PlayOneShot(Config.DogWoofScary);
        }
    }
    
    public void PlayPullAudio()
    {
        var randomChance = Random.Range(0f, 1f);
        if (randomChance < Config.DogWoofFriendlyOnPullChance)
        {
            audioSource.PlayOneShot(Config.DogWoofFriendly);
        }
    }

    private void FixedUpdate() {
        if (GameInput.GetDogMoveDir() != Vector3.zero) {
            Rigidbody.AddForce(GameInput.GetDogMoveDir() * Config.DogMoveForce);
        }

        if (GameInput.GetDogPush()) {
            PushRadius.Apply(Config.DogPushForce);
        }

        if (GameInput.GetDogPull()) {
            PullRadius.Apply(Config.DogPullForce);
        }
    }

    private void OnDrawGizmos() {
        var col = Gizmos.color;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Config.DogPullRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Config.DogPushRadius);

        Gizmos.color = col;
    }
}