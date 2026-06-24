using System.Collections.Generic;
using UnityEngine;

public class OvenRecipeMatcher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OvenController ovenController;
    [SerializeField] private List<OvenRecipeData> ovenRecipes;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    // Current dough item in the oven
    private GameObject currentDough;
    private string currentDoughTag = "";
    
    private void Start()
    {
        if (ovenController == null)
            ovenController = FindFirstObjectByType<OvenController>(); // Updated to FindFirstObjectByType
        
        if (ovenController != null)
        {
            ovenController.OnBakingComplete += CheckRecipeMatch;
            ovenController.OnBakingBurnt += OnBakingBurnt;
        }
    }
    
    public void PlaceDoughInOven(GameObject dough)
    {
        currentDough = dough;
        currentDoughTag = dough.tag;
        
        if (ovenController != null)
        {
            ovenController.PlaceDoughInOven(dough);
        }
        
        if (debugMode) Debug.Log($"Dough placed in oven: {dough.name}");
    }
    
    private void CheckRecipeMatch()
    {
        if (currentDough == null)
        {
            if (debugMode) Debug.Log("No dough in oven to check");
            return;
        }
        
        // Check if this dough matches any recipe
        foreach (OvenRecipeData recipe in ovenRecipes)
        {
            // Check if the dough tag matches the recipe's required ingredient
            // You can customize this matching logic based on your game's needs
            Dictionary<string, int> recipeDict = recipe.GetRecipeDictionary();
            
            foreach (var ingredient in recipeDict)
            {
                if (ingredient.Key == currentDoughTag)
                {
                    Debug.Log($"Recipe matched: {recipe.recipeName}! The {currentDoughTag} is perfectly baked!");
                    
                    // The oven controller will spawn the baked item
                    // You can add additional logic here
                    
                    return;
                }
            }
        }
        
        Debug.Log($"No recipe matched for: {currentDoughTag}");
    }
    
    private void OnBakingBurnt()
    {
        Debug.Log("The dough got burnt! Try again with shorter time or lower temperature.");
        
        // You can add visual feedback for burnt item here
        if (currentDough != null)
        {
            // Make it black or something
            Renderer rend = currentDough.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.black;
            }
        }
    }
    
    private void OnDestroy()
    {
        if (ovenController != null)
        {
            ovenController.OnBakingComplete -= CheckRecipeMatch;
            ovenController.OnBakingBurnt -= OnBakingBurnt;
        }
    }
}