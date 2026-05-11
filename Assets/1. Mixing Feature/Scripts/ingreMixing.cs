using UnityEngine;
using TMPro;

public class ingreMixing : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("TextMeshProUGUI component that will show the stacked ingredient list")]
    public TextMeshProUGUI outputText;

    [Header("Display options")]
    [Tooltip("Separator inserted between stacked ingredients")]
    public string separator = ", ";

    void Start()
    {
        if (outputText != null)
            outputText.text = string.Empty;
    }

    // Call this from Button onClick and pass the ingredient name as a string
    public void AddIngredient(string ingredient)
    {
        if (outputText == null) return;

        if (string.IsNullOrEmpty(outputText.text))
            outputText.text = ingredient;
        else
            outputText.text += separator + ingredient;
    }

    // Optional: clear the stacked ingredients (hook up a Clear button)
    public void ClearIngredients()
    {
        if (outputText == null) return;
        outputText.text = string.Empty;
    }
}
