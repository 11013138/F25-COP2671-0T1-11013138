using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightLighting : MonoBehaviour
{
    // variables
    public Light2D mainLight;
    public AnimationCurve lightIntensityCurve;
    public Gradient lightColorGradient;
    public float currentNormalizedTime = 0f; 
    public float currentLightFactor = 1f; 
    private TimeManager timeManager;

    public static DayNightLighting Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // try to find a TimeManager on gameobject, otherwise in scene
        timeManager = GetComponent<TimeManager>();
        if (timeManager == null)
        {
            timeManager = Object.FindFirstObjectByType<TimeManager>();
        }

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple DayNightLighting instances found. Keeping first.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (mainLight == null || timeManager == null)
        {
            return;
        }
        float normalizeTime = timeManager.currentHour / 24f;

        // intensity curve if present, otherwise fallback to day/night
        float evaluatedIntensity = 1f;
        if (lightIntensityCurve != null && lightIntensityCurve.length > 0)
        {
            evaluatedIntensity = lightIntensityCurve.Evaluate(normalizeTime);
        }
        else
        {
            evaluatedIntensity = (timeManager.currentHour >= 6f && timeManager.currentHour <= 18f) ? 1f : 0.2f;
        }

        // gradient color transitions
        Color evaluatedColor = Color.white;
        if (lightColorGradient != null)
        {
            evaluatedColor = lightColorGradient.Evaluate(normalizeTime);
        }
        else
        {
            evaluatedColor = (timeManager.currentHour >= 6f && timeManager.currentHour <= 18f) ? Color.white : new Color(0.5f, 0.5f, 1f, 1f);
        }

        // apply smoothly
        mainLight.intensity = Mathf.Lerp(mainLight.intensity, evaluatedIntensity, Time.deltaTime * 4f);
        mainLight.color = Color.Lerp(mainLight.color, evaluatedColor, Time.deltaTime * 4f);

        // normalized time and light factor
        currentNormalizedTime = normalizeTime;
        currentLightFactor = mainLight.intensity;
    }
}
