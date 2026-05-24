using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "BakersDream/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Basic Data")]
    public string recipeName;
    public int id;

    [Header("The resulting object that the recipe will create")]
    public IngredientsData ingredient;
    public GameObject itemPrefab;

    [Header("Required Ingredients")]
    public List<IngredientsData> reqIngredients;


    
}