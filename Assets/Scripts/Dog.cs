using UnityEngine;

public class Dog : MonoBehaviour {
    public GameConfig GameConfig;
    public GameInput GameInput;
    
    [Space]
    public Rigidbody Rigidbody;
    public DogPushRadius PushRadius;
    public DogPullRadius PullRadius;

    private void FixedUpdate() {
        if (GameInput.GetDogMoveDir() != Vector3.zero) {
            Rigidbody.AddForce(GameInput.GetDogMoveDir() * GameConfig.DogMoveForce);
        }

        if (GameInput.GetDogPush()) {
            PushRadius.Apply(GameConfig.DogPushForce);
        }

        if (GameInput.GetDogPull()) {
            PullRadius.Apply(GameConfig.DogPullForce);
        }
    }

    private void OnDrawGizmos() {
        var col = Gizmos.color;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GameConfig.DogPullRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, GameConfig.DogPushRadius);

        Gizmos.color = col;
    }
}