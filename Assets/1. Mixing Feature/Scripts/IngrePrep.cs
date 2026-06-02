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
                    print("Cup measure set to 50ml");
                    currentCup = CupMeasure.OneFifthcups;
                }
                else if (value == 2f)
                {
                    print("Cup measure set to 100ml");
                    currentCup = CupMeasure.Quartercups;
                }
                else if (value == 3f)
                {
                    print("Cup measure set to 150ml");
                    currentCup = CupMeasure.OneThirdcups;
                }
                else if (value == 4f)
                {
                    print("Cup measure set to 200ml");
                    currentCup = CupMeasure.Halfcups;
                }
                else if (value == 5f)
                {
                    print("Cup measure set to 250ml");
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
        print("Mouse type set to Whisk");
    }

    public void ScoopMode()
    {
        currentMouseType = MouseType.Scoop;
        print("Mouse type set to Scoop");
    }

    public void CupMode()
    {
        currentMouseType = MouseType.Cup;
        print("Mouse type set to Cup");
    }

    public void NormalMode()
    {
        currentMouseType = MouseType.Normal;
        print("Mouse type set to Normal");
    }

    public void ScaleMode()
    {
        currentMouseType = MouseType.Scale;
        print("Mouse type set to Scale");
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
            print($"Invalid scoop measure index: {measure}");
            return;
        }

        currentScoop = (ScoopMeasure)measure;
        print($"Scoop measure set to: {currentScoop}");
    }

    // Ingredient setter: single validated method for UI buttons (pass 1..n; 0 is None)
    public void SetIngredientWater()
    {
        if (currentMouseType == MouseType.Cup)
        {
            currentIngredient = IngredientType.Water;
            print("Ingredient set to: Water");
        }
        else
            print("Water can only be set in Cup mode");
    }

    public void SetIngredientOliveOil()
    {
        if (currentMouseType == MouseType.Cup)
        {
            currentIngredient = IngredientType.OliveOil;
            print("Ingredient set to: OliveOil");
        }
        else
            print("Olive Oil can only be set in Cup mode");
    }

    public void SetIngredientFlour()
    {
        if (currentMouseType == MouseType.Scoop)
        {
            currentIngredient = IngredientType.Flour;
            print("Ingredient set to: Flour");
        }
        else
            print("Flour can only be set in Scoop mode");
    }

    public void SetIngredientYeast()
    {
        if (currentMouseType == MouseType.Scoop)
        {
            currentIngredient = IngredientType.Yeast;
            print("Ingredient set to: Yeast");
        }
        else
            print("Yeast can only be set in Scoop mode");
    }

    public void SetIngredientSalt()
    {
        if (currentMouseType == MouseType.Scoop)
        {
            currentIngredient = IngredientType.Salt;
            print("Ingredient set to: Salt");
        }
        else
            print("Salt can only be set in Scoop mode");
    }

    public void SetIngredientSugar()
    {
        if (currentMouseType == MouseType.Scoop)
        {
            currentIngredient = IngredientType.Sugar;
            print("Ingredient set to: Sugar");
        }
        else
            print("Sugar can only be set in Scoop mode");
    }

    public void SetIngredientHoney()
    {
        if (currentMouseType == MouseType.Cup)
        {
            currentIngredient = IngredientType.Honey;
            print("Ingredient set to: Honey");
        }
        else
            print("Honey can only be set in Cup mode");
    }

    public void SetIngredientButter()
    {
        if (currentMouseType == MouseType.Scale)
        {
            currentIngredient = IngredientType.Butter;
            print("Ingredient set to: Butter");
        }
        else
            print("Butter can only be set in Scale mode");
    }

    public void SetIngredientMilk()
    {
        if (currentMouseType == MouseType.Cup)
        {
            currentIngredient = IngredientType.Milk;
            print("Ingredient set to: Milk");
        }
        else
            print("Milk can only be set in Cup mode");
    }

    public void SetIngredientEggs()
    {
        if (currentMouseType == MouseType.Normal)
        {
            currentIngredient = IngredientType.Eggs;
            print("Ingredient set to: Eggs");
        }
        else
            print("Eggs can only be set in Normal mode");
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
            print("No ingredient selected");
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
                    print($"{currentIngredient} must be added in Scoop mode");
                    return;
                }
                // Use enum index + 1 for a placeholder amount (Full=1, Half=2, etc. as requested)
                amount = (int)currentScoop + 1;
                print($"Measured {amount} scoop(s) of {key}");
                break;

            case IngredientType.Water:
            case IngredientType.OliveOil:
            case IngredientType.Honey:
            case IngredientType.Milk:
                // require Cup mode
                if (currentMouseType != MouseType.Cup)
                {
                    print($"{currentIngredient} must be added in Cup mode");
                    return;
                }
                // Use enum index + 1 for a placeholder amount (will replace with ml later)
                amount = (int)currentCup + 1;
                print($"Measured {amount} cup-measure(s) of {key}");
                break;

            case IngredientType.Butter:
                // require Scale mode
                if (currentMouseType != MouseType.Scale)
                {
                    print("Butter must be added in Scale mode");
                    return;
                }
                // Placeholder grams for now
                amount = 100;
                print($"Measured {amount} grams of {key}");
                break;

            case IngredientType.Eggs:
                // require Normal mode
                if (currentMouseType != MouseType.Normal)
                {
                    print("Eggs must be added in Normal mode");
                    return;
                }
                amount = 1;
                print($"Measured {amount} unit(s) of {key}");
                break;

            default:
                print("Selected ingredient is not supported for adding");
                return;
        }

        // Add or increment in the dictionary
        if (IngreList.ContainsKey(key))
        {
            IngreList[key] += amount;
            print($"Updated {key} -> {IngreList[key]}");
        }
        else
        {
            IngreList.Add(key, amount);
            print($"Added {key} -> {amount}");
        }
    }

    // Build and assign display string in the format: {Ingredient amount} per line
    private void UpdateIngreListDisplay()
    {
        if (IngreListText == null)
        {
            print("IngreListText (TMP) not assigned.");
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
        print("Ingredient list cleared");
    }
}
