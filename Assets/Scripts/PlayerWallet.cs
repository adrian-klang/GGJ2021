using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour {
    public GameConfig Config;
    
    private int TotalCoins = 0;

    public static PlayerWallet Instance;

    private AudioSource audioSource;
    
    private void Awake() {
        Instance = this;
    }

    private void OnDestroy() {
        Instance = null;
    }

    private void Start() {
        TotalCoins = Config.StartingCoinsCount;
        audioSource = GetComponent<AudioSource>();
    }

    public int GetTotalCoins() {
        return TotalCoins;
    }
    
    public void AddCoins(int amount) {
        TotalCoins += amount;
    }

    public void SpendCoins(int amount) {
        if (TotalCoins < amount) {
            Debug.LogError("Trying to spend more coins than the player has!");
            return;
        }

        audioSource.PlayOneShot(Config.CoinCling);
        TotalCoins -= amount;
    }
}
