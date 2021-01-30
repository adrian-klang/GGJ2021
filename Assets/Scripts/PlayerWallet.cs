using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour {
    public GameConfig Config;
    
    private int TotalCoins = 0;

    private void Start() {
        TotalCoins = Config.StartingCoinsCount;
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

        TotalCoins -= amount;
    }
}
