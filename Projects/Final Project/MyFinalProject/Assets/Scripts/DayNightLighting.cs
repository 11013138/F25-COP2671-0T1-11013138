using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightLighting : MonoBehaviour
{
    // variables
    public Light2D mainLight;
    public AnimationCurve lightIntensityCurve;
    public Gradient lightColorGradient;

    private TimeManager timeManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeManager = GetComponent<TimeManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (mainLight == null || timeManager == null)
        {
            return;
        }

        float normalizeTime = timeManager.currentHour/24f;

        // day = bright, night = dark
        if (lightIntensityCurve != null && lightIntensityCurve.length > 0)
        {
            mainLight.intensity = lightIntensityCurve.Evaluate(normalizeTime);
        }
        else
        {
            mainLight.intensity = (timeManager.currentHour >= 6f && timeManager.currentHour <= 18f) ? 1f : 0.2f;
        }

        // gradient color transitions
        if (lightColorGradient != null)
        {
            mainLight.color = lightColorGradient.Evaluate(normalizeTime);
        }
        else
        {
            mainLight.color = (timeManager.currentHour >= 6f && timeManager.currentHour <= 18f) ? Color.white : new Color(0.5f, 0.5f, 1f, 1f);
        }
    }
}
