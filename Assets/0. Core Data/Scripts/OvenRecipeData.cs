using UnityEngine;

[CreateAssetMenu(fileName = "New OvenRecipe", menuName = "BakersDream/OvenRecipe")]
public class OvenRecipeData : RecipeData
{
    [Header("Oven Settings (in Minutes)")]
    [Tooltip("Minimum baking time in MINUTES")]
    public float minTimer = 25f;
    [Tooltip("Maximum baking time in MINUTES")]
    public float maxTimer = 35f;
    [Tooltip("Minimum temperature in Celsius")]
    public float minTemperature = 170f;
    [Tooltip("Maximum temperature in Celsius")]
    public float maxTemperature = 190f;
    
    [Header("Visual Settings")]
    public Color bakedColor = new Color(0.83f, 0.63f, 0.08f);
}