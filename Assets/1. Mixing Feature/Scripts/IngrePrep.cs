using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngrePrep : MonoBehaviour
{
    [SerializeField] private Slider CupRuler;
    [SerializeField] private TextMeshProUGUI IngreListText; // assign in Inspector
    [SerializeField] private TextMeshProUGUI TipsPrompt; // assign in Inspector - displays tips/messages

    //Change MouseType
    public enum MouseType
    {
        Normal,
        Whisk,
        Scoop,
        Cup,
        Scale,
    }

    // change ScoopMeasure
    public enum ScoopMeasure
    {
        FullScoops,
        HalfScoops,
        OneThirdScoops,
        QuarterScoops,
        OneFifthScoops,
    }

    public enum CupMeasure
    {
        Fullcups,
        Halfcups,
        OneThirdcups,
        Quartercups,
        OneFifthcups,
    }

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
    }

    public float CupMeasurement;
    public MouseType currentMouseType = MouseType.Normal;
    public ScoopMeasure currentScoop = ScoopMeasure.FullScoops;
    public CupMeasure currentCup = CupMeasure.Fullcups;
    public IngredientType currentIngredient = IngredientType.None;
    Dictionary<string, int> IngreList = new Dictionary<string, int>();

    void Start()
    {
        if (CupRuler != null)
        {
            CupRuler.onValueChanged.AddListener((value) =>
            {
                if (value == 1f)
                {
                    ShowTip("Cup measure set to 50ml");
                    currentCup = CupMeasure.OneFifthcups;
                }
                else if (value == 2f)
                {
                    ShowTip("Cup measure set to 100ml");
                    currentCup = CupMeasure.Quartercups;
                }
                else if (value == 3f)
                {
                    ShowTip("Cup measure set to 150ml");
                    currentCup = CupMeasure.OneThirdcups;
                }
                else if (value == 4f)
                {
                    ShowTip("Cup measure set to 200ml");
                    currentCup = CupMeasure.Halfcups;
                }
                else if (value == 5f)
                {
                    ShowTip("Cup measure set to 250ml");
                    currentCup = CupMeasure.Fullcups;
                }
                // Optionally update UI or do something with the new cupMeasure value
            });
        }

        // initial display
        UpdateIngreListDisplay();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WhiskMode()
    {
        currentMouseType = MouseType.Whisk;
        ShowTip("Mouse type set to Whisk");
    }

    public void ScoopMode()
    {
        currentMouseType = MouseType.Scoop;
        ShowTip("Mouse type set to Scoop");
    }

    public void CupMode()
    {
        currentMouseType = MouseType.Cup;
        ShowTip("Mouse type set to Cup");
    }

    public void NormalMode()
    {
        currentMouseType = MouseType.Normal;
        ShowTip("Mouse type set to Normal");
    }

    public void ScaleMode()
    {
        currentMouseType = MouseType.Scale;
        ShowTip("Mouse type set to Scale");
    }

    // Helper to validate enum index
    private bool IsValidEnumIndex(System.Type enumType, int index)
    {
        if (!enumType.IsEnum) return false;
        return index >= 0 && index < System.Enum.GetValues(enumType).Length;
    }

    // Scoop setters: single validated method for UI buttons (pass 0..4)
    public void SetScoopMeasure(int measure)
    {
        if (!IsValidEnumIndex(typeof(ScoopMeasure), measure))
        {
            ShowTip($"Invalid scoop measure index: {measure}");
            return;
        }

        currentScoop = (ScoopMeasure)measure;
        ShowTip($"Scoop measure set to: {currentScoop}");
    }

    // Ingredient setter: single validated method for UI buttons (pass 1..n; 0 is None)
    public void SetIngredientWater()
    {
        if (currentMouseType == MouseType.Cup)
        {
            currentIngredient = IngredientType.Water;
            ShowTip("Ingredient set to: Water");
        }
        else
            ShowTip("Water can only be set in Cup mode");
    }

    public void SetIngredientOliveOil()
    {
        if (currentMouseType == MouseType.Cup)
        {
            currentIngredient = IngredientType.OliveOil;
            ShowTip("Ingredient set to: OliveOil");
        }
        else
            ShowTip("Olive Oil can only be set in Cup mode");
    }

    public void SetIngredientFlour()
    {
        if (currentMouseType == MouseType.Scoop)
        {
            currentIngredient = IngredientType.Flour;
            ShowTip("Ingredient set to: Flour");
        }
        else
            ShowTip("Flour can only be set in Scoop mode");
    }

    public void SetIngredientYeast()
    {
        if (currentMouseType == MouseType.Scoop)
        {
            currentIngredient = IngredientType.Yeast;
            ShowTip("Ingredient set to: Yeast");
        }
        else
            ShowTip("Yeast can only be set in Scoop mode");
    }

    public void SetIngredientSalt()
    {
        if (currentMouseType == MouseType.Scoop)
        {
            currentIngredient = IngredientType.Salt;
            ShowTip("Ingredient set to: Salt");
        }
        else
            ShowTip("Salt can only be set in Scoop mode");
    }

    public void SetIngredientSugar()
    {
        if (currentMouseType == MouseType.Scoop)
        {
            currentIngredient = IngredientType.Sugar;
            ShowTip("Ingredient set to: Sugar");
        }
        else
            ShowTip("Sugar can only be set in Scoop mode");
    }

    public void SetIngredientHoney()
    {
        if (currentMouseType == MouseType.Cup)
        {
            currentIngredient = IngredientType.Honey;
            ShowTip("Ingredient set to: Honey");
        }
        else
            ShowTip("Honey can only be set in Cup mode");
    }

    public void SetIngredientButter()
    {
        if (currentMouseType == MouseType.Scale)
        {
            currentIngredient = IngredientType.Butter;
            ShowTip("Ingredient set to: Butter");
        }
        else
            ShowTip("Butter can only be set in Scale mode");
    }

    public void SetIngredientMilk()
    {
        if (currentMouseType == MouseType.Cup)
        {
            currentIngredient = IngredientType.Milk;
            ShowTip("Ingredient set to: Milk");
        }
        else
            ShowTip("Milk can only be set in Cup mode");
    }

    public void SetIngredientEggs()
    {
        if (currentMouseType == MouseType.Normal)
        {
            currentIngredient = IngredientType.Eggs;
            ShowTip("Ingredient set to: Eggs");
        }
        else
            ShowTip("Eggs can only be set in Normal mode");
    }

    // Called by UI Button: adds current selection to the list and updates the TMP display
    public void inputIngre()
    {
        SetIngreList();
        UpdateIngreListDisplay();
    }

    public void SetIngreList()
    {
        // Adds or increments the selected ingredient in IngreList.
        // Rules (per request):
        // - Scoops: Flour, Yeast, Salt, Sugar  -> use currentScoop (use enum ordinal + 1 for now)
        // - Cups: Water, OliveOil, Honey, Milk -> use currentCup  (use enum ordinal + 1 for now)
        // - Scale: Butter                         -> placeholder grams (100)
        // - Normal: Eggs                          -> 1 unit
        // - Whisk: not used
        string key;
        int amount = 0;

        if (currentIngredient == IngredientType.None)
        {
            ShowTip("No ingredient selected");
            return;
        }

        key = currentIngredient.ToString();

        // Determine amount based on ingredient and active tool
        switch (currentIngredient)
        {
            case IngredientType.Flour:
            case IngredientType.Yeast:
            case IngredientType.Salt:
            case IngredientType.Sugar:
                // require Scoop mode
                if (currentMouseType != MouseType.Scoop)
                {
                    ShowTip($"{currentIngredient} must be added in Scoop mode");
                    return;
                }
                // Use enum index + 1 for a placeholder amount (Full=1, Half=2, etc. as requested)
                amount = (int)currentScoop + 1;
                ShowTip($"Measured {amount} scoop(s) of {key}");
                break;

            case IngredientType.Water:
            case IngredientType.OliveOil:
            case IngredientType.Honey:
            case IngredientType.Milk:
                // require Cup mode
                if (currentMouseType != MouseType.Cup)
                {
                    ShowTip($"{currentIngredient} must be added in Cup mode");
                    return;
                }
                // Use enum index + 1 for a placeholder amount (will replace with ml later)
                amount = (int)currentCup + 1;
                ShowTip($"Measured {amount} cup-measure(s) of {key}");
                break;

            case IngredientType.Butter:
                // require Scale mode
                if (currentMouseType != MouseType.Scale)
                {
                    ShowTip("Butter must be added in Scale mode");
                    return;
                }
                // Placeholder grams for now
                amount = 100;
                ShowTip($"Measured {amount} grams of {key}");
                break;

            case IngredientType.Eggs:
                // require Normal mode
                if (currentMouseType != MouseType.Normal)
                {
                    ShowTip("Eggs must be added in Normal mode");
                    return;
                }
                amount = 1;
                ShowTip($"Measured {amount} unit(s) of {key}");
                break;

            default:
                ShowTip("Selected ingredient is not supported for adding");
                return;
        }

        // Add or increment in the dictionary
        if (IngreList.ContainsKey(key))
        {
            IngreList[key] += amount;
            ShowTip($"Updated {key} -> {IngreList[key]}");
        }
        else
        {
            IngreList.Add(key, amount);
            ShowTip($"Added {key} -> {amount}");
        }
    }

    // Build and assign display string in the format: {Ingredient amount} per line
    private void UpdateIngreListDisplay()
    {
        if (IngreListText == null)
        {
            ShowTip("IngreListText (TMP) not assigned.");
            return;
        }

        if (IngreList.Count == 0)
        {
            IngreListText.text = "";
            return;
        }

        var sb = new StringBuilder();
        foreach (var kv in IngreList)
        {
            sb.Append(kv.Key);
            sb.Append(' ');
            sb.Append(kv.Value);

            // Determine unit text by ingredient key
            switch (kv.Key)
            {
                case "Flour":
                case "Yeast":
                case "Salt":
                case "Sugar":
                    sb.Append(" grams");
                    break;

                case "Water":
                case "OliveOil":
                case "Honey":
                case "Milk":
                    sb.Append(" ml");
                    break;

                case "Butter":
                    sb.Append(" grams");
                    break;

                case "Eggs":
                    sb.Append(" unit(s)");
                    break;

                default:
                    // leave without unit for unknown keys
                    break;
            }

            sb.AppendLine();
        }

        IngreListText.text = sb.ToString();
    }

    public void ClearIngreList()
    {
        IngreList.Clear();
        UpdateIngreListDisplay();
        ShowTip("Ingredient list cleared");
    }

    // Centralized UI tip writer. Falls back to Debug.Log if TipsPrompt not assigned.
    private void ShowTip(string message)
    {
        if (TipsPrompt != null)
            TipsPrompt.text = message;
        else
            Debug.Log(message);
    }
}
