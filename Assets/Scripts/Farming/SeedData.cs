using UnityEngine;
using Farming;
[CreateAssetMenu(fileName = "NewSeed", menuName = "Farming/SeedData")]
public class SeedData : ScriptableObject
{
    public string seedName;
    public int daysToMature = 3;
    public GameObject plantPrefab; 
    bool isSpecialSeed;

    public GameObject plantedModel;
    public GameObject growingModel;
    public GameObject matureModel;
    public GameObject whitheredModel;
    public GameObject specialPlantModel;
    public PlantType plantType;
    
}
