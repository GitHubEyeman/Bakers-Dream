using System.Collections;
using UnityEngine;
using UnityEngine.UI;   // for Slider
using TMPro;

public class OvenDropZone : DropZone
{
    [Header("Oven References")]
    [SerializeField] private OvenDoor ovenDoor;
    [SerializeField] private Transform itemSpawnPoint;

    [Header("Baking Settings")]
    [SerializeField] private OvenRecipeData[] recipes;      // <-- multiple recipes
    [SerializeField] private int currentTemperature = 180;   // can be changed via UI

    [Header("UI Elements")]
    [SerializeField] private GameObject bakingPanel;
    [SerializeField] private GameObject OvenDoorButton;
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Slider temperatureSlider;

    [Header("Fake Timer Settings")]
    [SerializeField] private float fakeBakeTimeMinutes = 5f;

    private bool isBaking = false;
    private GameObject currentItem;
    private OvenRecipeData activeRecipe;   // the recipe being used for the current bake

    void Start()
    {
        temperatureSlider.value = 50;
        SetTemperature(50);
    }

    public void SetTemperature(float temp)
    {
        currentTemperature = (int)temp;
        if (temperatureText != null)
            temperatureText.text = $"{currentTemperature}°C";
    }

    public void BakeItem(IngredientsData ingredientData, GameObject prefab)
    {
        if (prefab == null || isBaking) return;

        string ingredientName = ingredientData.ingredientName;

        // Find a recipe that requires this ingredient
        activeRecipe = null;
        foreach (var recipe in recipes)
        {
            var requiredDict = recipe.GetRecipeDictionary();
            if (requiredDict.ContainsKey(ingredientName))
            {
                activeRecipe = recipe;
                break;
            }
        }

        if (activeRecipe == null)
        {
            Debug.Log($"'{ingredientName}' is not required for any oven recipe.");
            return;
        }
        Debug.Log($"Baking {activeRecipe}");
        if (!ovenDoor.isOpen) ovenDoor.ToggleDoor();

        RemoveItemFromHotbar(ingredientName);

        currentItem = Instantiate(prefab, itemSpawnPoint.position, Quaternion.identity, transform);
        currentItem.transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp(currentItem, 0.3f));

        StartCoroutine(BakeProcess());
    }

    private IEnumerator ScaleUp(GameObject obj, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            obj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        obj.transform.localScale = Vector3.one;
    }

    private IEnumerator BakeProcess()
    {
        isBaking = true;
        temperatureSlider.interactable = false;
        if (OvenDoorButton != null) OvenDoorButton.SetActive(false);
        if (bakingPanel != null) bakingPanel.SetActive(true);

        if (ovenDoor != null)
        {
            yield return new WaitForSeconds(1.5f);
            if (ovenDoor.isOpen) ovenDoor.ToggleDoor();
            yield return new WaitForSeconds(0.6f);
        }

        // Use activeRecipe for timer
        float totalBakeTime = Random.Range(5, 10);
        float timeRemaining = totalBakeTime;

        if (temperatureText != null)
            temperatureText.text = $"{currentTemperature}°C";

        UpdateTimerDisplay(fakeBakeTimeMinutes * 60f);

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) timeRemaining = 0;

            float progress = 1f - (timeRemaining / totalBakeTime);
            float fakeRemaining = fakeBakeTimeMinutes * 60f * (1f - progress);
            UpdateTimerDisplay(fakeRemaining);

            yield return null;
        }

        UpdateTimerDisplay(0f);

        string resultQuality = EvaluateBakingResult(); // now uses activeRecipe

        GameObject resultPrefab = activeRecipe.ingredientResult?.itemPrefab;
        if (resultPrefab != null)
        {
            Debug.Log("resultPrefab is not NULL!");
            if (currentItem != null) Destroy(currentItem);

            GameObject bakedItem = Instantiate(resultPrefab, itemSpawnPoint.position, Quaternion.identity, transform);
            bakedItem.transform.localScale = Vector3.zero;
            StartCoroutine(ScaleUp(bakedItem, 0.3f));

            SaveManager.Instance.CurrentSave.AddToInventory(activeRecipe.ingredientResult.ingredientName);
        }

        Debug.Log($"Baking complete! Result: {resultQuality} (Temperature: {currentTemperature}°C, Real Time: {totalBakeTime}s, Displayed: {fakeBakeTimeMinutes}min)");

        if (ovenDoor != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (!ovenDoor.isOpen) ovenDoor.ToggleDoor();
        }

        if (bakingPanel != null) bakingPanel.SetActive(false);
        if (OvenDoorButton != null) OvenDoorButton.SetActive(true);

Debug.Log($"Baked {activeRecipe}");
        //MOVE TO THE NEXT SCENE
        yield return new WaitForSeconds(2.0f);
        EvaluateBakingResultToScene();






        isBaking = false;
        currentItem = null;
        activeRecipe = null; // clear the recipe reference
    }

    private void UpdateTimerDisplay(float totalSeconds)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private string EvaluateBakingResult()
    {
        // Use the activeRecipe's thresholds
        if (currentTemperature > activeRecipe.maxTemperature)
            return "Overcooked (burnt)";
        if (currentTemperature < activeRecipe.minTemperature)
            return "Undercooked (raw)";

        if (currentTemperature == activeRecipe.optimalTemperature)
            return "Perfect!";
        if (currentTemperature >= activeRecipe.minTemperature && currentTemperature <= activeRecipe.maxTemperature)
            return "Good (acceptable)";

        return "Unknown";
    }

    private string EvaluateBakingResultToScene()
    {
        System.String current = activeRecipe.ingredientResult.ingredientName;
        // Use the activeRecipe's thresholds
        if (currentTemperature > activeRecipe.maxTemperature)
            {
                SceneTransitioner.Instance.TriggerTransition("OvercookedBread");
                return "Overcooked (burnt)";}
        if (currentTemperature < activeRecipe.minTemperature)
            {
                SceneTransitioner.Instance.TriggerTransition("UndercookedBread");
                return "Undercooked (raw)";}

        if (currentTemperature == activeRecipe.optimalTemperature)
            {
                //HOW?
                switch (current.ToLowerInvariant())
                {
                    case "cavedinbread":
                        Debug.Log("cavedinbread");
                        SceneTransitioner.Instance.TriggerTransition("CavedInBread");
                        break;
                        
                    case "flatbread":
                        Debug.Log("flatbread");
                        SceneTransitioner.Instance.TriggerTransition("FlatBread");
                        break;
                                                
                    case "goldenbread":
                        Debug.Log("goldenbread");
                        SceneTransitioner.Instance.TriggerTransition("GBread");
                        break;
                                                
                    case "overcookedbread":
                        Debug.Log("overcookedbread");
                        SceneTransitioner.Instance.TriggerTransition("OvercookedBread");
                        break;
                                                
                    case "undercookedbread":
                        Debug.Log("undercookedbread");
                        SceneTransitioner.Instance.TriggerTransition("UndercookedBread");
                        break;
                        
                    default:
                        Debug.Log("Item not recognized.");
                        SceneTransitioner.Instance.TriggerTransition("");
                        break;
                }
                return "Perfect!";}
        if (currentTemperature >= activeRecipe.minTemperature && currentTemperature <= activeRecipe.maxTemperature)
            {
                //Same as above
                switch (current.ToLowerInvariant())
                {
                    case "cavedinbread":
                        Debug.Log("cavedinbread");
                        SceneTransitioner.Instance.TriggerTransition("CavedInBread");
                        break;
                        
                    case "flatbread":
                        Debug.Log("flatbread");
                        SceneTransitioner.Instance.TriggerTransition("FlatBread");
                        break;
                                                
                    case "goldenbread":
                        Debug.Log("goldenbread");
                        SceneTransitioner.Instance.TriggerTransition("GBread");
                        break;
                                                
                    case "overcookedbread":
                        Debug.Log("overcookedbread");
                        SceneTransitioner.Instance.TriggerTransition("OvercookedBread");
                        break;
                                                
                    case "undercookedbread":
                        Debug.Log("undercookedbread");
                        SceneTransitioner.Instance.TriggerTransition("UndercookedBread");
                        break;
                        
                    default:
                        Debug.Log("Item not recognized.");
                        SceneTransitioner.Instance.TriggerTransition("");
                        break;
                }
                //CavedInBread

                return "Good (acceptable)";}

        return "Unknown";
    }
}