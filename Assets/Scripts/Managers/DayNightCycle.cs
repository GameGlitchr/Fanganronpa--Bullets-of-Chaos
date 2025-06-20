using UnityEngine;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    public GameObject Sun;
    public TextMeshProUGUI clockText;
    public bool use24HourClock = false;

    public float fullDayDuration = 900f;  // 15 minutes total
    private float dayDuration = 600f;     // 10 minutes
    private float nightDuration = 300f;   // 5 minutes

    private float daySpeed;
    private float nightSpeed;
    private float currentRotation = 0f;

    void Start()
    {
        daySpeed = 240f / dayDuration;   // 240 degrees over 10 mins
        nightSpeed = 120f / nightDuration; // 120 degrees over 5 mins
    }

    void Update()
    {
        if (PlayerManager.instance != null && PlayerManager.instance.isPaused)
            return;

        // Determine current time period
        float speed = currentRotation < 240f ? daySpeed : nightSpeed;

        // Advance sun rotation
        currentRotation += speed * Time.deltaTime;
        if (currentRotation >= 360f)
        {
            currentRotation -= 360f;
        }

        Sun.transform.rotation = Quaternion.Euler(currentRotation, 0f, 0f);

        UpdateClock();
    }

    void UpdateClock()
    {
        // Normalize sun rotation to 0–1
        float normalizedTime = currentRotation / 360f;

        // Convert to hours and shift by +6 to align 0° = 6:00 AM
        float totalHours = (normalizedTime * 24f + 6f) % 24f;
        int hour = Mathf.FloorToInt(totalHours);
        int minute = Mathf.FloorToInt((totalHours - hour) * 60f);

        if (use24HourClock)
        {
            clockText.text = $"{hour:00}:{minute:00}";
        }
        else
        {
            string period = hour >= 12 ? "PM" : "AM";
            int twelveHour = hour % 12;
            if (twelveHour == 0) twelveHour = 12;
            clockText.text = $"{twelveHour:00}:{minute:00} {period}";
        }
    }
}
