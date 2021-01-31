using UnityEngine;

public class GameInput : MonoBehaviour
{
    public enum InputState {
        Game,
        Placement
    }

    private InputState state = InputState.Game;

    private bool dogPull = false;
    private bool dogPush = false;
    private Vector3 moveDog = Vector3.zero;

    private void Awake() {
        state = InputState.Game;
    }

    public Vector3 GetDogMoveDir() {
        return moveDog;
    }

    public bool GetDogPull() {
        return dogPull;
    }

    public bool GetDogPush() {
        return dogPush;
    }

    public void SetState(InputState newState) {
        state = newState;
    }

    public bool GetValidatePlacement() {
        if (state != InputState.Placement) {
            return false;
        }
        
        return Input.GetMouseButtonDown(0);
    }

    public bool RotatePlacement() {
        if (state != InputState.Placement) {
            return false;
        }

        return Input.mouseScrollDelta != Vector2.zero;
    }

    private void Update() {
        moveDog = Vector3.zero;
        
        // Down
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            moveDog.z = -1;
        }
        
        // Up
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
            moveDog.z = 1;
        }
        
        // Left
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            moveDog.x = -1;
        }
        
        // Right
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            moveDog.x = 1;
        }
        
        // Exit
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (state == InputState.Game) {
            // Pull
            if (Input.GetMouseButtonDown(0)) {
                dogPull = true;
            }
        
            if (Input.GetMouseButtonUp(0)) {
                dogPull = false;
            }
        
            // Push
            if (Input.GetMouseButtonDown(1)) {
                dogPush = true;
            }

            if (Input.GetMouseButtonUp(1)) {
                dogPush = false;
            }
        } else {
            dogPull = false;
            dogPush = false;
        }

        if (state == InputState.Placement) {
            
        }
    }
}
