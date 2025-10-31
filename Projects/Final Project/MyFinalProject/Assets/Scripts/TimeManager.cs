using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    // variables
    public float currentHour = 6f;
    public float currentDay = 1;

    public UnityEvent OnSunrise;
    public UnityEvent OnSunset;
    public UnityEvent OnNewDay;

    private bool sunrise = false;
    private bool sunset = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnSunrise = new UnityEvent();
        OnSunset = new UnityEvent();
        OnNewDay = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        // time moves forward (240 seconds = 1 day)
        currentHour += Time.deltaTime * (24f / 240f);

        // new day starts at midnight
        if (currentHour >= 24f)
        {
            currentHour = 0f;
            currentDay++;

            // reset at midnight
            sunrise = false;
            sunset = false;

            OnNewDay.Invoke();
        }

        // sunrise starts at 6AM
        if (currentHour >= 6f && currentHour < 18f && !sunrise)
        {
            sunrise = true;
            sunset = false;
            OnSunrise.Invoke();
        }

        // sunset starts at 6PM
        if (currentHour >= 18f && !sunset)
        {
            sunset = true;
            sunrise = false;
            OnSunset.Invoke();
        }

    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 30), "Day " + currentDay + " Hour: " + currentHour.ToString("F1"));
    }
}
