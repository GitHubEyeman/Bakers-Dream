using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "BakersDream/Ingredients")]
public class IngredientsData : ScriptableObject
{
    [Header("Basic Data")]
    public string ingredientName;
    
    //must be unique
    public int id; 

    //Set to false if only this is an ingredient that was made by mixing or kneading or oven baked, ect...
    public bool isBasicIngredients = true; 



    [Header("Item Model")]
    public GameObject itemPrefab;
}
