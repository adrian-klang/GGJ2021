using UnityEngine;

public class DayNightCycle : MonoBehaviour {
    public GameConfig Config;
    public Light Light;

    [Space]
    [ColorUsage(false, true)]
    public Color DayTopColor;
    [ColorUsage(false, true)]
    public Color DayMiddleColor;
    [ColorUsage(false, true)]
    public Color DayBottomColor;

    [Space]
    [ColorUsage(false, true)]
    public Color NightTopColor;
    [ColorUsage(false, true)]
    public Color NightMiddleColor;
    [ColorUsage(false, true)]
    public Color NightBottomColor;

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

        var colorT = DayNightProgress / Config.DayNightThreshold;
        if (colorT <= 1.0f) {
            RenderSettings.ambientSkyColor = Color.Lerp(DayTopColor, NightTopColor, colorT);
            RenderSettings.ambientEquatorColor = Color.Lerp(DayMiddleColor, NightMiddleColor, colorT);
            RenderSettings.ambientGroundColor = Color.Lerp(DayBottomColor, NightBottomColor, colorT);
            Light.intensity = 1.0f - colorT * 0.8f;
        } else {
            var extendedNight = Config.DayNightThreshold + 0.2f;
            colorT = Mathf.Clamp01((DayNightProgress - extendedNight) / (1.0f - extendedNight));
            RenderSettings.ambientSkyColor = Color.Lerp(NightTopColor, DayTopColor, colorT);
            RenderSettings.ambientEquatorColor = Color.Lerp(NightMiddleColor, DayMiddleColor, colorT);
            RenderSettings.ambientGroundColor = Color.Lerp(NightBottomColor, DayBottomColor, colorT);
            Light.intensity = 0.2f + colorT * 0.8f;
        }
        DynamicGI.UpdateEnvironment();

        // Light.transform.eulerAngles = new Vector3(Config.DayNightLightCurve.Evaluate(DayNightProgress) * 360, -30, 0);
    }
}