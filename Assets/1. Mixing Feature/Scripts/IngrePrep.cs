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
        Cup
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
    public ScoopMeasure currentScoop = ScoopMeasure.FullScoop;
    public CupMeasure currentCup = CupMeasure.FullCup;
    public IngredientType currentIngredient = IngredientType.None;

    void Start()
    {
        CupRuler.onValueChanged.AddListener((value) =>
        {
            if (value == 1f)
                print("Cup measure set to 50ml");
                    currentCup = CupMeasure.OneFifthcups;
            else if (value == 2f)
                print($"Cup measure set to 100m1");
                    currentCup = CupMeasure.Quartercups;
            else if (value == 3f)
                print($"Cup measure set to 150ml");
                    currentCup = CupMeasure.OneThirdcups;
            else if (value == 4f)
                print($"Cup measure set to 200ml");
                    currentCup = CupMeasure.Halfcups,;
            else if (value == 5f)
                print($"Cup measure set to 250ml");
                    currentCup = CupMeasure.FullCup;
            // Optionally update UI or do something with the new cupMeasure value
        });
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

    public void SetScoopMeasure(int measure)
    {
        currentScoop = (ScoopMeasure)measure;
        print($"Scoop measure set to: {currentScoop}");
    }
}
