using System;
using System.Text;
using Core;
using TMPro;
using UnityEngine;

namespace Environment
{
    public class SeasonManager : MonoBehaviour
    {
        public static SeasonManager Instance { get; private set; }

        public enum DayOfWeek
        {
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday,
            Count
        }

        public enum Season
        {
            Winter,
            Spring,
            Summer,
            Fall,
            Count
        }

        public enum TimeOfDay
        {
            Night,
            Dawn,
            Morning,
            Afternoon,
            Evening
        }

        private static readonly string[] SeasonNames = Enum.GetNames(typeof(Season));
        private static readonly string[] DayNames = Enum.GetNames(typeof(DayOfWeek));

        [Header("Scene References")]
        [SerializeField] private Light sunLight;
        [SerializeField] private TMP_Text seasonLabel;
        [SerializeField] private SeasonalParticleController particleController;

        [Header("Season Data")]
        [SerializeField] private SeasonData[] seasons = new SeasonData[(int)Season.Count];
        [SerializeField] private SeasonData spring;
        [SerializeField] private SeasonData summer;
        [SerializeField] private SeasonData fall;
        [SerializeField] private SeasonData winter;

        [Header("Calendar")]
        [SerializeField] private Season startingSeason = Season.Spring;
        [SerializeField] private DayOfWeek startingDay = DayOfWeek.Monday;
        [SerializeField] private int startingDayNumber = 1;
        [SerializeField] private int daysPerSeason = 7;

        [Header("Runtime State")]
        [SerializeField] private SeasonData currentSeason;
        [SerializeField] private Season currentSeasonType;
        [SerializeField] private DayOfWeek currentDayOfWeek;
        [SerializeField] private TimeOfDay currentTimeOfDay;
        [SerializeField] private int currentDayNumber = 1;
        [SerializeField] [Range(0f, 1f)] private float timeOfDayProgress;

        private SeasonData scratchData;
        private readonly StringBuilder labelBuilder = new StringBuilder(64);

        public Season CurrentSeason => currentSeasonType;
        public DayOfWeek CurrentDay => currentDayOfWeek;
        public TimeOfDay CurrentTime => currentTimeOfDay;
        public int CurrentDayNumber => currentDayNumber;

        public SeasonData RuntimeData
        {
            get => scratchData;
            private set
            {
                scratchData = value != null ? Instantiate(value) : null;
                currentSeason = scratchData;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EnsureReferences();
            SyncSeasonArray();
            InitializeCalendar();
            ApplySeasonData(true);
            RefreshLabel();
            ApplySunState();
        }

        private void OnValidate()
        {
            if (seasons == null || seasons.Length != (int)Season.Count)
            {
                Array.Resize(ref seasons, (int)Season.Count);
            }

            startingDayNumber = Mathf.Max(1, startingDayNumber);
            daysPerSeason = Mathf.Max(1, daysPerSeason);
            SyncSeasonArray();
        }

        public void SetSeason(Season season)
        {
            currentSeasonType = season;
            ApplySeasonData(true);
            RefreshLabel();
            ApplySunState();
        }

        public void AdvanceDay()
        {
            currentDayNumber++;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetDay(currentDayNumber);
            }

            SyncCalendarFromDayNumber();
            ApplySeasonData();
            SetTimeOfDayProgress(0f);
            RefreshLabel();
        }

        public void SetTimeOfDayProgress(float normalizedProgress)
        {
            timeOfDayProgress = Mathf.Clamp01(normalizedProgress);
            currentTimeOfDay = ResolveTimeOfDay(timeOfDayProgress);
            ApplySunState();
        }

        public void RefreshLabel()
        {
            if (seasonLabel == null)
            {
                return;
            }

            labelBuilder.Clear();
            labelBuilder.Append(SeasonNames[(int)currentSeasonType])
                .Append(" - ")
                .Append(DayNames[(int)currentDayOfWeek])
                .Append(" Day: ")
                .Append(currentDayNumber);
            seasonLabel.SetText(labelBuilder);
        }

        private void InitializeCalendar()
        {
            currentDayNumber = Mathf.Max(1,
                GameManager.Instance != null ? GameManager.Instance.currentDay : startingDayNumber);

            SyncCalendarFromDayNumber();
            timeOfDayProgress = 0f;
            currentTimeOfDay = ResolveTimeOfDay(timeOfDayProgress);
        }

        private void SyncCalendarFromDayNumber()
        {
            int elapsedDays = Mathf.Max(0, currentDayNumber - startingDayNumber);
            currentDayOfWeek = (DayOfWeek)(((int)startingDay + elapsedDays) % (int)DayOfWeek.Count);
            currentSeasonType = (Season)(((int)startingSeason + (elapsedDays / daysPerSeason)) % (int)Season.Count);
        }

        private void ApplySeasonData(bool forceParticles = false)
        {
            SeasonData seasonData = seasons[(int)currentSeasonType];
            RuntimeData = seasonData;

            if (particleController != null && (forceParticles || particleController.CurrentSeason != currentSeasonType))
            {
                particleController.UpdateSeason(currentSeasonType);
            }
        }

        private void ApplySunState()
        {
            if (sunLight == null)
            {
                return;
            }

            float rotationX = Mathf.Lerp(-20f, 200f, timeOfDayProgress);
            sunLight.transform.rotation = Quaternion.Euler(rotationX, 35f, 0f);

            Color targetColor = RuntimeData != null ? RuntimeData.sunColor : Color.white;
            float daylightAmount = Mathf.Clamp01(1f - Mathf.Abs((timeOfDayProgress * 2f) - 1f));
            sunLight.intensity = Mathf.Lerp(0.2f, 1.1f, daylightAmount);
            sunLight.color = Color.Lerp(targetColor * 0.45f, targetColor, daylightAmount);
        }

        private TimeOfDay ResolveTimeOfDay(float normalizedProgress)
        {
            if (normalizedProgress < 0.15f)
            {
                return TimeOfDay.Dawn;
            }

            if (normalizedProgress < 0.4f)
            {
                return TimeOfDay.Morning;
            }

            if (normalizedProgress < 0.7f)
            {
                return TimeOfDay.Afternoon;
            }

            if (normalizedProgress < 0.9f)
            {
                return TimeOfDay.Evening;
            }

            return TimeOfDay.Night;
        }

        private void EnsureReferences()
        {
            if (sunLight == null)
            {
                sunLight = FindFirstObjectByType<Light>();
            }

            if (seasonLabel == null)
            {
                seasonLabel = GameObject.Find("DayLabel")?.GetComponent<TMP_Text>();
            }

            if (particleController == null)
            {
                particleController = FindFirstObjectByType<SeasonalParticleController>();
            }
        }

        private void SyncSeasonArray()
        {
            seasons[(int)Season.Winter] = winter != null ? winter : seasons[(int)Season.Winter];
            seasons[(int)Season.Spring] = spring != null ? spring : seasons[(int)Season.Spring];
            seasons[(int)Season.Summer] = summer != null ? summer : seasons[(int)Season.Summer];
            seasons[(int)Season.Fall] = fall != null ? fall : seasons[(int)Season.Fall];
        }
    }
}