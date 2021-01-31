using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dog : MonoBehaviour {
    public GameConfig Config;
    public GameInput GameInput;
    
    [Space]
    public Rigidbody Rigidbody;
    public DogPushRadius PushRadius;
    public DogPullRadius PullRadius;

    public Animator Animator;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayScaryAudio(float chance)
    {
        var randomChance = Random.Range(0f, 1f);
        if (randomChance < chance)
        {
            if (audioSource.clip != Config.DogWoofScary) {
                audioSource.Stop();
            }
        
            if (!audioSource.isPlaying) {
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.clip = Config.DogWoofScary;
                audioSource.Play();
            }
        }
    }
    
    public void PlayFriendlyAudio()
    {
        var randomChance = Random.Range(0f, 1f);
        if (randomChance < Config.DogWoofFriendlyOnPullChance) {
            if (audioSource.clip != Config.DogWoofFriendly) {
                audioSource.Stop();
            }
        
            if (!audioSource.isPlaying) {
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.clip = Config.DogWoofFriendly;
                audioSource.Play();
            }
        }
    }

    private void Update() {
        if (GameInput.GetDogMoveDir() != Vector3.zero) {
            Animator.SetBool("Walk", true);
            transform.forward = Rigidbody.velocity.normalized;
        } else {
            Animator.SetBool("Walk", false);
        }
    }

    private void FixedUpdate() {
        if (GameInput.GetDogMoveDir() != Vector3.zero) {
            Rigidbody.AddForce(GameInput.GetDogMoveDir() * Config.DogMoveForce);
        }

        if (GameInput.GetDogPush()) {
            Animator.SetTrigger("Bark");
            PushRadius.Apply(Config.DogPushForce);
        }

        if (GameInput.GetDogPull()) {
            Animator.SetTrigger("Bark");
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