using UnityEngine;

[CreateAssetMenu(fileName = "New OvenRecipe", menuName = "BakersDream/OvenRecipe")]
public class OvenRecipeData : RecipeData
{
    [Header("Oven Settings")]
    public int minTemperature;    // Minimum temperature treshold in Celsius/Fahrenheit to get good results
    public int maxTemperature;    // Maximum temperature treshold in Celsius/Fahrenheit to get good results
    public int optimalTemperature; // The temperature that gives the best results


}