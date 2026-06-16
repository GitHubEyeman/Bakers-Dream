using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    public RecipeData recipe;
    public RecipeUIPanel recipeUIPanel;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (recipeUIPanel.panel.activeSelf)
        {
            recipeUIPanel.HidePanel();
        }
        else
        {
            recipeUIPanel.ShowRecipe(recipe);
        }
    }
}