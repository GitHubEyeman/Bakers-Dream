using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "BakersDream/Ingredients")]
public class IngredientsData : ScriptableObject
{
    [Header("Basic Data")]
    public string ingredientName;
    public int id; //must be unique
    [Header("Item Model")]
    public GameObject itemPrefab;
}
