using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        var wolf = other.gameObject.GetComponent<Wolf>();
        if (wolf == null) {
            return;
        }
        
        Destroy(wolf);
        Destroy(gameObject);
    }
}
