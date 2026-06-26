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
    [SerializeField] private OvenRecipeData recipe;
    [SerializeField] private int currentTemperature = 180;   // can be changed via UI

    [Header("UI Elements")]
    [SerializeField] private GameObject bakingPanel;        // parent panel to show/hide
    [SerializeField] private GameObject OvenDoorButton;
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Slider temperatureSlider;      // optional – assign if you have one

    [Header("Fake Timer Settings")]
    [SerializeField] private float fakeBakeTimeMinutes = 5f; // total displayed baking time (minutes)

    private bool isBaking = false;
    private GameObject currentItem;

    void Start()
    {
        temperatureSlider.value = 50;
        SetTemperature(50);
    }

    // Call this from your temperature slider's OnValueChanged event
    public void SetTemperature(float temp)
    {
        currentTemperature = (int)temp;
        if (temperatureText != null)
            temperatureText.text = $"{currentTemperature}°C";
    }

    // Called directly from the UI slot
    public void BakeItem(IngredientsData ingredientData, GameObject prefab)
    {
        if (prefab == null || isBaking) return;

        string ingredientName = ingredientData.ingredientName;
        var requiredDict = recipe.GetRecipeDictionary();
        if (!requiredDict.ContainsKey(ingredientName))
        {
            Debug.Log($"'{ingredientName}' is not required for this recipe.");
            return;
        }
        if (!ovenDoor.isOpen) ovenDoor.ToggleDoor();

        // Remove from hotbar (rebuilds the hotbar and destroys this slot)
        RemoveItemFromHotbar(ingredientName);

        // Spawn the dough inside the oven
        currentItem = Instantiate(prefab, itemSpawnPoint.position, Quaternion.identity, transform);
        currentItem.transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp(currentItem, 0.3f));

        // Start the baking coroutine
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
        // Show the baking UI panel
        if (bakingPanel != null) bakingPanel.SetActive(true);

        // Close door if assigned
        if (ovenDoor != null)
        {
            yield return new WaitForSeconds(1.5f);
            if (ovenDoor.isOpen) ovenDoor.ToggleDoor();
            yield return new WaitForSeconds(0.6f); // match your close duration
        }

        // Determine total baking time (random between min and max) – this is the REAL time (in seconds)
        float totalBakeTime = Random.Range(5, 10);
        float timeRemaining = totalBakeTime;

        // Update temperature display once
        if (temperatureText != null)
            temperatureText.text = $"{currentTemperature}°C";

        // Set initial fake timer to full fake time (e.g., "05:00")
        UpdateTimerDisplay(fakeBakeTimeMinutes * 60f);

        // Baking loop – update timer every frame
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            // Clamp to zero
            if (timeRemaining < 0) timeRemaining = 0;

            // Compute fake remaining time based on progress (0 -> 1)
            float progress = 1f - (timeRemaining / totalBakeTime); // 0 at start, 1 at end
            float fakeRemaining = fakeBakeTimeMinutes * 60f * (1f - progress);
            UpdateTimerDisplay(fakeRemaining);

            yield return null;
        }

        // Baking finished – show "00:00"
        UpdateTimerDisplay(0f);

        // Determine result quality based on temperature
        string resultQuality = EvaluateBakingResult();

        // Produce the final item
        GameObject resultPrefab = recipe.ingredientResult?.itemPrefab;
        if (resultPrefab != null)
        {
            if (currentItem != null) Destroy(currentItem);

            GameObject bakedItem = Instantiate(resultPrefab, itemSpawnPoint.position, Quaternion.identity, transform);
            bakedItem.transform.localScale = Vector3.zero;
            StartCoroutine(ScaleUp(bakedItem, 0.3f));

            // Add to inventory (the baked item)
            SaveManager.Instance.CurrentSave.AddToInventory(recipe.ingredientResult.ingredientName);
        }

        // Debug the result (shows real time)
        Debug.Log($"Baking complete! Result: {resultQuality} (Temperature: {currentTemperature}°C, Real Time: {totalBakeTime}s, Displayed: {fakeBakeTimeMinutes}min)");

        // Open door again
        if (ovenDoor != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (!ovenDoor.isOpen) ovenDoor.ToggleDoor();
        }

        // Hide the baking UI
        if (bakingPanel != null) bakingPanel.SetActive(false);
        if (OvenDoorButton != null) OvenDoorButton.SetActive(true);

        isBaking = false;
        currentItem = null;
    }

    // Helper to format and update the timer text
    private void UpdateTimerDisplay(float totalSeconds)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Evaluates the baking quality based on the current temperature
    private string EvaluateBakingResult()
    {
        if (currentTemperature > recipe.maxTemperature)
            return "Overcooked (burnt)";
        if (currentTemperature < recipe.minTemperature)
            return "Undercooked (raw)";

        if (currentTemperature == recipe.optimalTemperature)
            return "Perfect!";
        if (currentTemperature >= recipe.minTemperature && currentTemperature <= recipe.maxTemperature)
            return "Good (acceptable)";

        return "Unknown";
    }
}