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

    private Coroutine messageCoroutine;
    
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
        
        
    }
    
    public void OnBuyTrap() {
        if (Wallet.GetTotalCoins() < Config.WolfTrapValue) {
            return;
        }
        
        
    }

    public void Update() {
        CoinsCountText.text = Wallet.GetTotalCoins().ToString();
        FenceValueText.text = Config.FenceValue.ToString();
        WolfTrapValueText.text = Config.WolfTrapValue.ToString();
    }
    
    private IEnumerator MessageRoutine(string text) {
        MessageText.text = text;
        MessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(Config.MessageTextTimeout);
        MessageText.gameObject.SetActive(false);
    }
}
