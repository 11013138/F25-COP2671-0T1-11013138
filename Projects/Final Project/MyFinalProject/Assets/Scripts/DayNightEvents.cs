using UnityEngine;

public class DayNightEvents : MonoBehaviour
{
    // variables
    private TimeManager timeManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeManager = GetComponent<TimeManager>();

        timeManager.OnSunrise.AddListener(Sunrise);
        timeManager.OnSunset.AddListener(Sunset);
        timeManager.OnNewDay.AddListener(OnNewDay);

    }

    void Sunrise()
    {
        Debug.Log("Sunrise!");
    }

    void Sunset()
    {
        Debug.Log("Sunset!");
    }

    void OnNewDay()
    {
        Debug.Log("Today is a New Day!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
