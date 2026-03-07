using UnityEngine;
using Farming;
using Environment;
using Core;

public class Plant : MonoBehaviour
{
    public enum PlantState {Planted, Growing, Mature, Whithered}
    public PlantState currentState = PlantState.Planted;
    
    /*
    [SerializeField] private GameObject plantedModel;
    [SerializeField] private GameObject growingModel;
    [SerializeField] private GameObject matureModel;
    [SerializeField] private GameObject whitheredModel;
    */

    private int dayGrown = 0;
    private int daysToMature = 3;
    public SeedData seedData;
    [SerializeField] private Transform modelHolder;
    private GameObject currentModel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateVisual();
    }
    public void PlantSeed(SeedData selectSeed)
    {
        seedData  = selectSeed;
        currentState = PlantState.Planted;
        dayGrown = 0;
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
        /*
        //Hide All Model
        plantedModel.SetActive(false);
        growingModel.SetActive(false);
        matureModel.SetActive(false);
        whitheredModel.SetActive(false);
        */
        if(currentModel != null)
        {
            Destroy(currentModel);
        }
        GameObject prefabtoSpawn = null;
        //Show the model based on the current state
        switch (currentState)
        {
            case PlantState.Planted: prefabtoSpawn = seedData.plantedModel;break;
            case PlantState.Growing: prefabtoSpawn = seedData.growingModel; break;
            case PlantState.Mature: prefabtoSpawn = seedData.matureModel; break;
            case PlantState.Whithered: prefabtoSpawn = seedData.whitheredModel; break;
        }
        if (prefabtoSpawn != null)
            currentModel = Instantiate(prefabtoSpawn, modelHolder);
    }

    public bool IsMature()
    {
        return currentState == PlantState.Mature;
    }
}
