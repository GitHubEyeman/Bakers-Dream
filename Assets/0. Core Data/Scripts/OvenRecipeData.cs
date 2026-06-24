using UnityEngine;

[CreateAssetMenu(fileName = "New OvenRecipe", menuName = "BakersDream/OvenRecipe")]
public class OvenRecipeData : RecipeData
{
    [Header("Oven Settings")]
    [Tooltip("Minimum baking time in seconds")]
    public float minTimer = 20f;
    [Tooltip("Maximum baking time in seconds")]
    public float maxTimer = 40f;
    [Tooltip("Minimum temperature in Celsius")]
    public float minTemperature = 150f;
    [Tooltip("Maximum temperature in Celsius")]
    public float maxTemperature = 250f;
    
    [Header("Visual Settings")]
    [Tooltip("Color of the baked item when done")]
    public Color bakedColor = new Color(0.85f, 0.65f, 0.13f); // Golden color (R:218, G:165, B:32)
}