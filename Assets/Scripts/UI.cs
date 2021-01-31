using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
    public Text CoinsCountText;
    public Text FenceValueText;
    public Text WolfTrapValueText;
    public PlayerWallet Wallet;
    public GameConfig Config;
    public Text MessageText;
    public GameInput GameInput;
    public Camera Cam;

    private Coroutine messageCoroutine;
    private GameObject currentPlacementInstance;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    
    public void ShowMessage(string text) {
        if (messageCoroutine != null) {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(MessageRoutine(text));
    }

    public void OnBuyFence() {
        if (Wallet.GetTotalCoins() < Config.FenceValue) {
            return;
        }
        
        Wallet.SpendCoins(Config.FenceValue);
        GameInput.SetState(GameInput.InputState.Placement);
        SetPlacementInstance(Config.Fence);
    }
    
    public void OnBuyTrap() {
        if (Wallet.GetTotalCoins() < Config.WolfTrapValue) {
            return;
        }
        
        Wallet.SpendCoins(Config.WolfTrapValue);
        GameInput.SetState(GameInput.InputState.Placement);
        SetPlacementInstance(Config.Trap);
    }

    public void Update() {
        CoinsCountText.text = Wallet.GetTotalCoins().ToString();
        FenceValueText.text = Config.FenceValue.ToString();
        WolfTrapValueText.text = Config.WolfTrapValue.ToString();

        if (currentPlacementInstance != null) {
            var ray = Cam.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float dist)) {
                var point = ray.GetPoint(dist);

                currentPlacementInstance.transform.position = point;

                if (GameInput.RotatePlacement()) {
                    currentPlacementInstance.transform.eulerAngles = currentPlacementInstance.transform.eulerAngles + new Vector3(0, 90, 0);
                }

                if (GameInput.GetValidatePlacement()) {
                    PlaceInstance();
                    GameInput.SetState(GameInput.InputState.Game);
                }
            }
        }
    }

    private void SetPlacementInstance(GameObject prefab) {
        if (currentPlacementInstance != null) {
            Destroy(currentPlacementInstance);
        }

        currentPlacementInstance = Instantiate(prefab);

        foreach (var r in currentPlacementInstance.GetComponentsInChildren<Collider>()) {
            r.enabled = false;
        }
    }

    private void PlaceInstance() {
        foreach (var r in currentPlacementInstance.GetComponentsInChildren<Collider>()) {
            r.enabled = true;
        }
        
        currentPlacementInstance = null;
    }
    
    private IEnumerator MessageRoutine(string text) {
        MessageText.text = text;
        MessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(Config.MessageTextTimeout);
        MessageText.gameObject.SetActive(false);
    }
}
