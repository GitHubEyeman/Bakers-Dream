using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "BakersDream/Recipe")]
public class RecipeData : ScriptableObject
{
    [Serializable]
    public struct IngredientPair
    {
        public string itemName;
        public int amount;
    }
    [Header("Basic Data")]
    public string recipeName;
    public int id;

    [Header("The resulting object that the recipe will create")]
    public IngredientsData ingredient;
    public GameObject itemPrefab;

    [Header("Required Ingredients")]
    [SerializeField] private List<IngredientPair> requiredIngredients = new List<IngredientPair>();
    public Dictionary<string, int> GetRecipeDictionary()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        foreach (var pair in requiredIngredients)
        {
            if (!dict.ContainsKey(pair.itemName))
            {
                dict.Add(pair.itemName, pair.amount);
            }
        }
        return dict;
    }

    


    
}

