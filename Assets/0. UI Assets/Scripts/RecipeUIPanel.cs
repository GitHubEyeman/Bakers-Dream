using UnityEngine;
using TMPro;

public class RecipeUIPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI ingredientsText;

    public void ShowRecipe(RecipeData recipe)
    {
        panel.SetActive(true);

        recipeNameText.text = recipe.recipeName;

        string ingredientList = "Required Ingredients:\n";
        foreach (IngredientsData ingredient in recipe.reqIngredients)
        {
            ingredientList += $"- {ingredient.ingredientName}\n";
        }
        ingredientsText.text = ingredientList;
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}