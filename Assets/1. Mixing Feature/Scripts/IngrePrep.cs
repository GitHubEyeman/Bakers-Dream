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
        QuarterScoop,
        HalfScoop,
        FullScoop,
        HeapingScoop,
        Tablespoon,
        Teaspoon,
        Custom
    }
    public float CupMeasurement;
    public MouseType currentMouseType = MouseType.Normal;
    public ScoopMeasure currentScoop = ScoopMeasure.FullScoop;

    void Start()
    {
        CupRuler.onValueChanged.AddListener((value) =>
        {
            CupMeasurement = value;
            print($"Cup measure updated to: {CupMeasurement} cups");
            // Optionally update UI or do something with the new cupMeasure value
        });
    }

    // Update is called once per frame
    void Update()
    {

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
}
