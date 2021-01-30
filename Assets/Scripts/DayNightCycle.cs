using UnityEngine;

public class DayNightCycle : MonoBehaviour {
    public GameConfig Config;
    
    // Day - 0 - 0.5, Night - 0.5 - 1
    public static float DayNightValue = 0;    
    
    public static bool IsItNight() {
        return DayNightValue > 0.5f;
    }

    public static bool IsItDay() {
        return DayNightValue <= 0.5f;
    }

    public void Update() {
        DayNightValue += Time.deltaTime * Config.DayNightCycleSpeed;
        DayNightValue %= 1.0f;
    }
}
