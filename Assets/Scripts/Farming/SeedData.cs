using Environment;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSeed", menuName = "Farming/SeedData")]
public class SeedData : ScriptableObject
{
    public string seedName;
    public int daysToMature = 3;
    public GameObject plantPrefab; 

    public GameObject plantedModel;
    public GameObject growingModel;
    public GameObject matureModel;
    public GameObject whitheredModel;

    [Header("Season Availability")]
    public bool availableInAllSeasons = true;
    public SeasonManager.Season[] allowedSeasons;

    public bool IsAvailableInSeason(SeasonManager.Season season)
    {
        if (availableInAllSeasons || allowedSeasons == null || allowedSeasons.Length == 0)
        {
            return true;
        }

        foreach (SeasonManager.Season allowedSeason in allowedSeasons)
        {
            if (allowedSeason == season)
            {
                return true;
            }
        }

        return false;
    }

    public string GetSeasonSummary()
    {
        if (availableInAllSeasons || allowedSeasons == null || allowedSeasons.Length == 0)
        {
            return "All Seasons";
        }

        return string.Join(", ", allowedSeasons);
    }
}
