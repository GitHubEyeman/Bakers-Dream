using UnityEngine;
using TMPro;

public class ingreMixing : MonoBehaviour
{
    // Which tool/mode the "mouse" (tool) is using
    public enum MouseType
    {
        Normal,
        Whisk,
        Scoop,
        Cup
    }

    // Keep ScoopMeasure the same
    public enum ScoopMeasure
    {
        QuarterScoop,
        HalfScoop,
        FullScoop,
        HeapingScoop,
        Tablespoon,
        Teaspoon,
        Custom
    }

    [Header("UI")]
    [Tooltip("TextMeshProUGUI component that will show the stacked ingredient list")]
    public TextMeshProUGUI outputText;

    [Header("Display options")]
    [Tooltip("Separator inserted between stacked ingredients")]
    public string separator = ", ";

    [Header("Tool / Measurement defaults")]
    [Tooltip("Current mouse/tool type")]
    public MouseType currentMouseType = MouseType.Normal;
    [Tooltip("Default scoop measure used when MouseType is Scoop")]
    public ScoopMeasure currentScoop = ScoopMeasure.FullScoop;

    [Header("Cup measurement (use a slider in Inspector)")]
    [Tooltip("Amount in cups; shown in inspector as a slider")]
    [Range(0f, 4f)]
    public float cupMeasure = 1f;

    void Start()
    {
        if (outputText != null)
            outputText.text = string.Empty;
    }

    // Simple existing API - unchanged
    public void AddIngredient(string ingredient)
    {
        if (outputText == null) return;

        if (string.IsNullOrEmpty(outputText.text))
            outputText.text = ingredient;
        else
            outputText.text += separator + ingredient;
    }

    // New: combine MouseType with measurement. 
    // If MouseType == Scoop, uses the provided scoop measure & count.
    // If MouseType == Cup, uses the provided cupAmount (otherwise inspector's cupMeasure).
    // If MouseType == Whisk, marks ingredient as "whisked".
    // If Normal, adds ingredient plainly.
    public void AddIngredient(string ingredient, MouseType mouseType, ScoopMeasure scoop = ScoopMeasure.FullScoop, int scoopCount = 1, float cupAmount = -1f)
    {
        if (outputText == null) return;

        string display = ingredient;

        // choose cupAmount: if caller passes negative, use inspector slider value
        float effectiveCup = cupAmount < 0f ? cupMeasure : cupAmount;

        // Combine MouseType and measurement
        if (mouseType == MouseType.Scoop)
        {
            string scoopText = FormatScoop(scoop, scoopCount);
            display = $"{scoopText} {ingredient}";
        }
        else if (mouseType == MouseType.Cup)
        {
            string cupText = FormatCupValue(effectiveCup);
            display = $"{cupText} {ingredient}";
        }
        else if (mouseType == MouseType.Whisk)
        {
            // Whisk could change attribute; here we mark the ingredient as whisked
            display = $"whisked {ingredient}";
        }
        else // Normal or fallback
        {
            display = ingredient;
        }

        AddIngredient(display);
    }

    // Optional: clear the stacked ingredients (hook up a Clear button)
    public void ClearIngredients()
    {
        if (outputText == null) return;
        outputText.text = string.Empty;
    }

    // Helper formatting methods
    private string FormatScoop(ScoopMeasure scoop, int count)
    {
        string scoopName = scoop.ToString();
        if (scoop == ScoopMeasure.Custom)
            scoopName = "custom scoop";

        if (count <= 1)
            return $"{scoopName}";
        return $"{count} x {scoopName}";
    }

    private string FormatCupValue(float amount)
    {
        // singular/plural and formatting
        string qty = Mathf.Approximately(amount, Mathf.Round(amount))
            ? Mathf.RoundToInt(amount).ToString()
            : amount.ToString("0.##");

        string unit = Mathf.Approximately(amount, 1f) ? "cup" : "cups";
        return $"{qty} {unit}";
    }
}
