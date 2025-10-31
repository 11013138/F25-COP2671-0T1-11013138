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
        mainLight = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainLight == null)
        {
            return;
        }

        float hour = timeManager.currentHour;

        // day = bright, night = dark
        if (hour >= 6f && hour < 18f)
        {
            mainLight.intensity = 1f;
            mainLight.color = Color.white;
        }

        else
        {
            mainLight.intensity = 0.3f;
            mainLight.color = new Color(0.3f, 0.3f, 0.6f); // blue color at night
        }
    }
}
