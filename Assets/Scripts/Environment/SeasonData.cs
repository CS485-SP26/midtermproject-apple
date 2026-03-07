using UnityEngine;

namespace Environment
{
    [CreateAssetMenu(fileName = "SeasonData", menuName = "Scriptable Objects/SeasonData")]
    
    public class SeasonData : ScriptableObject
    {
        [Range(0f, 24f)]
        [Tooltip("Daylight hours")]
        public float dayLength;

        [Range(-40f, 140f)]
        [Tooltip("Temperature in Fahrenheit")]
        public float avgTemp;

        public Color sunColor;
    }
}