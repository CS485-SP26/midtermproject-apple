using UnityEngine;
using Farming;
using Environment;

public class Plant : MonoBehaviour
{
    public enum PlantState {Planted, Growing, Mature, Whithered}
    public PlantState currentState = PlantState.Planted;

    [SerializeField] private GameObject plantedModel;
    [SerializeField] private GameObject growingModel;
    [SerializeField] private GameObject matureModel;
    [SerializeField] private GameObject whitheredModel;

    [SerializeField] private SeasonManager seasonManager; // call instances

    private int dayGrown = 0;
    private int daysToMature = 3;
    private int maxWitherLevel = 3;
    private int witherLevel = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateVisual();
    }
    
    public void OnDayPassed(bool wasWatered)
    {
        if(currentState == PlantState.Mature || currentState == PlantState.Whithered)
            return;
        if (!wasWatered)
        {
            currentState = PlantState.Whithered;
            UpdateVisual();
            return;
        }
        dayGrown++;
        if(dayGrown >= daysToMature)
        {
            currentState = PlantState.Mature;
        }
        else
        {
            currentState = PlantState.Growing;
        }
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        //Hide All Model
        plantedModel.SetActive(false);
        growingModel.SetActive(false);
        matureModel.SetActive(false);
        whitheredModel.SetActive(false);
        //Show the model based on the current state
        switch (currentState)
        {
            case PlantState.Planted: plantedModel.SetActive(true);break;
            case PlantState.Growing: growingModel.SetActive(true); break;
            case PlantState.Mature: matureModel.SetActive(true); break;
            case PlantState.Whithered: whitheredModel.SetActive(true); break;
        }
    }

    public bool IsMature()
    {
        return currentState == PlantState.Mature;
    }


    
    // this will set the witherrate based on the season. should this be called in to change the daysToMature??
    private int getSeasonWither()
    {
        var currentSeason = seasonManager.GetCurrentSeason();

        switch (currentSeason)
        {
            case Season.SeasonType.Spring: 
                return 2;
            case Season.SeasonType.Summer: 
                return 1;
            case Season.SeasonType.Autumn: 
                return 1;
            case Season.SeasonType.Winter: 
                return 0;
            default: 
                return 1;
        }
    }

    
}
