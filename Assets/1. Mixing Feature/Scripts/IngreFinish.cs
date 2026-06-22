using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IngreFinish : MonoBehaviour
{
    [SerializeField] private IngrePrep ingrePrep;
    [SerializeField] private List<RecipeData> RecipeDatas;
    private Dictionary<string, int> IngreList;

    void Start()
    {
        IngreList = ingrePrep.IngreList;
    }

    public void OnClickButton()
    {
        bool recipeFound = false;

        // Loop through every recipe you assigned in the inspector
        foreach (RecipeData recipe in RecipeDatas)
        {
            Dictionary<string, int> recipeDict = recipe.GetRecipeDictionary();

            // Match condition: Same count, and every key-value pair matches perfectly
            bool isExactMatch = IngreList.Count == recipeDict.Count && 
                               !IngreList.Except(recipeDict).Any();

            if (isExactMatch)
            {
                // Debug out every item inside the successfully matched recipe
                foreach (KeyValuePair<string, int> item in IngreList)
                {
                    Debug.Log($"Success! Matched Recipe '{recipe.name}' -> {item.Key}: {item.Value}");
                    //TODO: Add the resulting stuff to Inventory Later
                }
                
                recipeFound = true;
                
                break; // Stop looking since we found the exact match
            }
        }

        // If the loop finished and no recipe matched the user's ingredients perfectly
        if (!recipeFound)
        {
            Debug.Log("Not found");
        }
    }
}
