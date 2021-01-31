using UnityEngine;

public class DayNightCycle : MonoBehaviour {
    public GameConfig Config;
    public GameObject Light;
    
    public static float DayNightProgress = 0;
    public static int DayCounter;
    
    public bool IsItNight() {
        return DayNightProgress > Config.DayNightThreshold;
    }

    public bool IsItDay() {
        return DayNightProgress <= Config.DayNightThreshold;
    }

    public void Update() {
        DayNightProgress += Time.deltaTime / Config.DayNightCycleDurationInSeconds;
        DayNightProgress %= 1.0f;

        Light.transform.eulerAngles = new Vector3(Config.DayNightLightCurve.Evaluate(DayNightProgress) * 360, -30, 0);
    }
}
