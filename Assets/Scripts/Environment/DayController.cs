using UnityEngine;
using TMPro; // Important for TextMeshPro
using UnityEngine.Events;
using Core;


namespace Environment 
{
    public class DayController : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private Light sunLight;
        [SerializeField] private TMP_Text dayLabel;
        [SerializeField] private SeasonManager seasonManager;
        
        [Header("Time Constraints")]
        [SerializeField] private float dayLengthSeconds = 60f;
        [SerializeField] private float dayProgressSeconds = 0f;
        private bool dayAlreadyAdvancedThisCycle = false;

        public float DayProgressPercent => dayLengthSeconds <= 0f ? 0f : Mathf.Clamp01(dayProgressSeconds / dayLengthSeconds);
        public int CurrentDay => seasonManager != null ? seasonManager.CurrentDayNumber : GameManager.Instance.currentDay;

        public UnityEvent dayPassedEvent = new UnityEvent();

        private void Awake()
        {
            dayProgressSeconds = 0f;

            if (seasonManager == null)
            {
                seasonManager = FindFirstObjectByType<SeasonManager>();
            }

            if (sunLight == null)
            {
                sunLight = FindFirstObjectByType<Light>();
            }

            if (seasonManager != null)
            {
                seasonManager.RefreshLabel();
            }
            else if (GameManager.Instance != null)
            {
                dayLabel?.SetText("Days: {0}", GameManager.Instance.currentDay);
            }
        }

        public void AdvanceDay()
        {
            dayProgressSeconds = 0f;

            if (seasonManager != null)
            {
                seasonManager.AdvanceDay();
            }
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.currentDay++;
                GameManager.Instance.SetDay(GameManager.Instance.currentDay);
                dayLabel?.SetText("Days: {0}", GameManager.Instance.currentDay);
            }

            dayPassedEvent.Invoke();
        }

        public void UpdateVisuals()
        {
            if (seasonManager != null)
            {
                seasonManager.SetTimeOfDayProgress(DayProgressPercent);
                return;
            }

            if (sunLight == null)
            {
                return;
            }

            float sunRotationX = Mathf.Lerp(0f, 360f, DayProgressPercent);
            sunLight.transform.rotation = Quaternion.Euler(sunRotationX, 0f, 0f);
        }

        private void Update()
        {
            dayProgressSeconds += Time.deltaTime;

            if (dayProgressSeconds >= dayLengthSeconds && !dayAlreadyAdvancedThisCycle)
            {
                AdvanceDay();
                dayAlreadyAdvancedThisCycle = true;
            }

            if (dayProgressSeconds < dayLengthSeconds)
            {
                dayAlreadyAdvancedThisCycle = false;
            }

            UpdateVisuals();
        }
    }
}