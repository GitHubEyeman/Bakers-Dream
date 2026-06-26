using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "BakersDream/Recipe")]
public class RecipeData : ScriptableObject
{
    public enum IngredientType
    {
        None,
        Water,
        OliveOil,
        Flour,
        Yeast,
        Salt,
        Sugar,
        Honey,
        Butter,
        Milk,
        Eggs,
        Dough,
        GDough,
        UnderCookedDough,
        FlatBrickDough,
        CavedInDough,
        OverCookedDough,
        CavedInBread,
        FlatBread,
        GoldenBread,
        OvercookedBread,
        UndercookedBread,
        GDough2,
        UnderCookedDough2,
        FlatBrickDough2,
        CavedInDough2,
        OverCookedDough2,
        

    }

    [Serializable]
    public struct IngredientPair
    {
        public IngredientType itemName;
        public int amount;
    }
    [Header("Basic Data")]
    public string recipeName;
    public int id;

    [Header("The resulting object that the recipe will create")]
    public IngredientsData ingredientResult;
    public GameObject itemPrefab;

    [Header("Required Ingredients")]
    [SerializeField] private List<IngredientPair> requiredIngredients = new List<IngredientPair>();
    public Dictionary<string, int> GetRecipeDictionary()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        foreach (var pair in requiredIngredients)
        {
            if (!dict.ContainsKey(pair.itemName.ToString()))
            {
                dict.Add(pair.itemName.ToString(), pair.amount);
            }
        }
        return dict;
    }

    


    
}

