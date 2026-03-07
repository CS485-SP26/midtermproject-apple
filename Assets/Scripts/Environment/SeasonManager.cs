using UnityEngine;

namespace Environment
{
    public class SeasonManager : MonoBehaviour
    {
        public enum Season
        {
            Winter,
            Spring,
            Summer,
            Fall,
            Count
        }
        [SerializeField] private SeasonData[] seasons = new SeasonData[(int)Season.Count];


        [SerializeField] private SeasonData spring;
        [SerializeField] private SeasonData summer;
        [SerializeField] private SeasonData fall;
        [SerializeField] private SeasonData winter;

        [SerializeField] private SeasonData currentSeason;

        private SeasonData scratchData;
        private Season currentSeasonType = Season.Spring;

        // <summary>
        // safe to edit this data without destroying the original ScriptableObject asset, but it will not persist after exiting play mode
        // </summary>

        public SeasonData RuntimeData
        {
            get
            {
                return scratchData;
            }
            set
            {
                // create a safe clone of the data
                scratchData = Instantiate(value);
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetSeason(Season.Spring);

        }

        public void SetSeason(Season value)
        {
            currentSeasonType = value;
            RuntimeData = seasons[(int)value];
        }

        public Season GetCurrentSeason()
        {
            return currentSeasonType;
        }
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}