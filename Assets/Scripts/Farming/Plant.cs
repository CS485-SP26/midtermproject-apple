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

    private int dayGrown = 0;
    private int daysToMature = 3;
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
        //Debug.Log("The plant is mature and ready to harvest!");
    }
}
