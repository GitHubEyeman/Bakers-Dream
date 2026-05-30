using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngrePrep : MonoBehaviour
{
    [SerializeField] private Slider CupRuler;

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
        if (currentMouseType == MouseType.Scoop
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
}
