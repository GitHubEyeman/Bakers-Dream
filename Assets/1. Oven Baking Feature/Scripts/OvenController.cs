using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class OvenController : MonoBehaviour
{
    [Header("Oven Settings")]
    [SerializeField] private float minTemperature = 150f;
    [SerializeField] private float maxTemperature = 300f;
    [SerializeField] private float defaultTemperature = 180f;
    [SerializeField] private float temperatureStep = 5f;
    
    [Header("Timer Settings (Minutes in UI)")]
    [SerializeField] private float minBakeTimeMinutes = 10f;
    [SerializeField] private float maxBakeTimeMinutes = 60f;
    [SerializeField] private float defaultBakeTimeMinutes = 30f;
    [SerializeField] private float timeStepMinutes = 1f;
    
    [Header("Game Speed Settings")]
    [SerializeField] private float secondsPerBakingMinute = 1f;
    
    [Header("UI References")]
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI temperatureDisplay;
    [SerializeField] private TextMeshProUGUI timeDisplay;
    [SerializeField] private TextMeshProUGUI statusDisplay;
    [SerializeField] private GameObject ovenDoor;
    [SerializeField] private GameObject bakingTray;
    [SerializeField] private GameObject bakedItemPrefab;
    [SerializeField] private GameObject burntItemPrefab; // Optional: burnt version
    
    [Header("Door Settings")]
    [SerializeField] private float doorOpenAngle = -100f;
    [SerializeField] private float doorCloseAngle = 0f;
    [SerializeField] private float doorSpeed = 3f;
    [SerializeField] private GameObject doorPivot;
    
    [Header("Baking Visuals")]
    [SerializeField] private Material doughMaterial;
    [SerializeField] private Color rawColor = new Color(0.96f, 0.90f, 0.83f);
    [SerializeField] private Color bakingColor = new Color(1f, 0.84f, 0f);
    [SerializeField] private Color perfectColor = new Color(0.83f, 0.63f, 0.08f);
    [SerializeField] private Color overdoneColor = new Color(0.5f, 0.3f, 0.1f);
    [SerializeField] private Color burntColor = new Color(0.1f, 0.1f, 0.1f);
    [SerializeField] private Color underdoneColor = new Color(0.9f, 0.85f, 0.75f);
    
    [Header("Scoring")]
    [SerializeField] private float perfectScore = 100f;
    [SerializeField] private float goodScore = 75f;
    [SerializeField] private float undercookedScore = 50f;
    [SerializeField] private float overcookedScore = 40f;
    [SerializeField] private float burntScore = 20f;
    
    [Header("Audio")]
    [SerializeField] private string ovenStartSFX = "ovenStart";
    [SerializeField] private string ovenTickSFX = "ovenTick";
    [SerializeField] private string ovenDoneSFX = "ovenDone";
    [SerializeField] private string ovenBurntSFX = "ovenBurnt";
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    // Private Variables
    private float currentTemperature;
    private float currentBakeTimeMinutes;
    private float currentBakeTimeSeconds;
    private float elapsedSeconds = 0f;
    private bool isBaking = false;
    private bool isOvenOpen = false;
    private Coroutine bakingCoroutine;
    private Coroutine temperatureAnimationCoroutine;
    
    private bool isDoorAnimating = false;
    private Quaternion doorTargetRotation;
    
    private RecipeData currentRecipe;
    private bool bakingCompleted = false;
    
    // Recipe reference values
    private float recipeMinTemp = 170f;
    private float recipeMaxTemp = 190f;
    private float recipeMinTimeMinutes = 25f;
    private float recipeMaxTimeMinutes = 35f;
    private float recipeIdealTemp = 180f;
    private float recipeIdealTimeMinutes = 30f;
    
    public event Action<float> OnBakingProgress;
    public event Action<string, float> OnBakingComplete;
    
    // Properties
    public bool IsBaking => isBaking;
    public float CurrentTemperature => currentTemperature;
    public float ElapsedTime => elapsedSeconds;
    public float TotalBakeTimeSeconds => currentBakeTimeSeconds;
    public float TotalBakeTimeMinutes => currentBakeTimeMinutes;
    public bool IsBakingCompleted => bakingCompleted;
    
    private void Start()
    {
        currentTemperature = defaultTemperature;
        currentBakeTimeMinutes = defaultBakeTimeMinutes;
        currentBakeTimeSeconds = ConvertMinutesToSeconds(currentBakeTimeMinutes);
        
        SetupUI();
        SetupDoor();
        SetupDough();
        
        CheckForDoughFromGameManager();
        CheckForRecipeFromGameManager();
        
        Debug.Log($"=== OVEN INITIALIZED ===");
        Debug.Log($"Default time: {currentBakeTimeMinutes} min = {currentBakeTimeSeconds}s");
        Debug.Log($"==========================");
    }
    
    private void SetupUI()
    {
        if (temperatureSlider != null)
        {
            temperatureSlider.minValue = minTemperature;
            temperatureSlider.maxValue = maxTemperature;
            temperatureSlider.value = currentTemperature;
            temperatureSlider.onValueChanged.AddListener(OnTemperatureChanged);
        }
        
        if (timeSlider != null)
        {
            timeSlider.minValue = minBakeTimeMinutes;
            timeSlider.maxValue = maxBakeTimeMinutes;
            timeSlider.value = currentBakeTimeMinutes;
            timeSlider.onValueChanged.AddListener(OnTimeChanged);
        }
        
        UpdateDisplay();
        SetStatus("Ready to bake!");
    }
    
    private void SetupDoor()
    {
        if (doorPivot == null && ovenDoor != null)
        {
            doorPivot = ovenDoor.transform.Find("DoorPivot")?.gameObject;
            if (doorPivot == null) doorPivot = ovenDoor;
        }
        
        if (doorPivot != null)
        {
            doorPivot.transform.localRotation = Quaternion.Euler(doorCloseAngle, 0, 0);
        }
        isOvenOpen = false;
    }
    
    private void SetupDough()
    {
        if (doughMaterial != null)
        {
            doughMaterial.color = rawColor;
        }
    }
    
    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.bKey.wasPressedThisFrame) ToggleBaking();
            if (Keyboard.current.oKey.wasPressedThisFrame) ToggleOvenDoor();
        }
        
        if (isDoorAnimating && doorPivot != null)
        {
            doorPivot.transform.localRotation = Quaternion.Slerp(
                doorPivot.transform.localRotation,
                doorTargetRotation,
                Time.deltaTime * doorSpeed
            );
            
            if (Quaternion.Angle(doorPivot.transform.localRotation, doorTargetRotation) < 0.5f)
            {
                doorPivot.transform.localRotation = doorTargetRotation;
                isDoorAnimating = false;
            }
        }
    }
    
    private float ConvertMinutesToSeconds(float minutes)
    {
        return minutes * secondsPerBakingMinute;
    }
    
    private float ConvertSecondsToMinutes(float seconds)
    {
        return seconds / secondsPerBakingMinute;
    }
    
    private void CheckForDoughFromGameManager()
    {
        if (GameManager.Instance != null && GameManager.Instance.hasDoughReady)
        {
            GameObject dough = GameManager.Instance.GetDough();
            if (dough != null)
            {
                PlaceDoughInOven(dough);
                SetStatus("Dough received from kneading!");
                
                if (doughMaterial != null)
                {
                    float quality = GameManager.Instance.doughQuality;
                    Color startColor = Color.Lerp(rawColor, bakingColor, quality * 0.3f);
                    doughMaterial.color = startColor;
                    Debug.Log($"Dough placed with quality: {quality:F2}");
                }
            }
        }
        else
        {
            Debug.Log("No dough from GameManager. Using default dough.");
        }
    }
    
    private void CheckForRecipeFromGameManager()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.currentRecipeName))
        {
            string recipeName = GameManager.Instance.currentRecipeName;
            LoadRecipe(recipeName);
        }
        else
        {
            Debug.Log("No recipe from GameManager. Using default recipe values.");
            // Set default recipe values for testing
            recipeMinTemp = 170f;
            recipeMaxTemp = 190f;
            recipeIdealTemp = 180f;
            recipeMinTimeMinutes = 25f;
            recipeMaxTimeMinutes = 35f;
            recipeIdealTimeMinutes = 30f;
        }
    }
    
    public void LoadRecipe(string recipeName)
    {
        RecipeData[] allRecipes = Resources.LoadAll<RecipeData>("");
        foreach (RecipeData recipe in allRecipes)
        {
            if (recipe.recipeName == recipeName)
            {
                currentRecipe = recipe;
                SetOvenFromRecipe(recipe);
                SetStatus($"Recipe loaded: {recipeName}");
                Debug.Log($"Recipe loaded: {recipeName}");
                return;
            }
        }
        
        Debug.LogWarning($"Recipe not found: {recipeName}");
    }
    
    public void SetOvenFromRecipe(RecipeData recipe)
    {
        OvenRecipeData ovenRecipe = recipe as OvenRecipeData;
        if (ovenRecipe != null)
        {
            // Store recipe values
            recipeMinTemp = ovenRecipe.minTemperature;
            recipeMaxTemp = ovenRecipe.maxTemperature;
            recipeIdealTemp = (ovenRecipe.minTemperature + ovenRecipe.maxTemperature) / 2f;
            recipeMinTimeMinutes = ovenRecipe.minTimer;
            recipeMaxTimeMinutes = ovenRecipe.maxTimer;
            recipeIdealTimeMinutes = (ovenRecipe.minTimer + ovenRecipe.maxTimer) / 2f;
            
            // Set oven to ideal recipe values
            currentTemperature = recipeIdealTemp;
            currentBakeTimeMinutes = recipeIdealTimeMinutes;
            currentBakeTimeSeconds = ConvertMinutesToSeconds(currentBakeTimeMinutes);
            
            // Update UI
            if (temperatureSlider != null) temperatureSlider.value = currentTemperature;
            if (timeSlider != null) timeSlider.value = currentBakeTimeMinutes;
            
            UpdateDisplay();
            
            Debug.Log($"=== RECIPE LOADED ===");
            Debug.Log($"Ideal Temp: {recipeIdealTemp}°C (Range: {recipeMinTemp}°C - {recipeMaxTemp}°C)");
            Debug.Log($"Ideal Time: {recipeIdealTimeMinutes} min (Range: {recipeMinTimeMinutes} - {recipeMaxTimeMinutes} min)");
            Debug.Log($"Oven set to: {currentTemperature}°C, {currentBakeTimeMinutes} min");
            Debug.Log($"====================");
        }
    }
    
    public void OnTemperatureChanged(float value)
    {
        if (isBaking) return;
        currentTemperature = Mathf.Round(value / temperatureStep) * temperatureStep;
        UpdateDisplay();
        if (debugMode) Debug.Log($"Temperature set to: {currentTemperature}°C");
    }
    
    public void OnTimeChanged(float value)
    {
        if (isBaking) return;
        currentBakeTimeMinutes = Mathf.Round(value / timeStepMinutes) * timeStepMinutes;
        currentBakeTimeSeconds = ConvertMinutesToSeconds(currentBakeTimeMinutes);
        UpdateDisplay();
        Debug.Log($"Time set to: {currentBakeTimeMinutes} minutes ({currentBakeTimeSeconds}s)");
    }
    
    public void ToggleBaking()
    {
        if (!isBaking) StartBaking();
        else StopBaking();
    }
    
    public void StartBaking()
    {
        if (isBaking) return;
        if (isOvenOpen)
        {
            SetStatus("Close the oven door first!");
            return;
        }
        
        elapsedSeconds = 0f;
        isBaking = true;
        bakingCompleted = false;
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(ovenStartSFX);
        
        if (bakingCoroutine != null) StopCoroutine(bakingCoroutine);
        bakingCoroutine = StartCoroutine(BakingProcess());
        
        SetStatus($"Baking... ({currentBakeTimeMinutes} min)");
        
        Debug.Log($"=== BAKING STARTED ===");
        Debug.Log($"Temp: {currentTemperature}°C (Recipe: {recipeIdealTemp}°C)");
        Debug.Log($"Time: {currentBakeTimeMinutes} min (Recipe: {recipeIdealTimeMinutes} min)");
        Debug.Log($"=====================");
        
        if (temperatureAnimationCoroutine != null) StopCoroutine(temperatureAnimationCoroutine);
        temperatureAnimationCoroutine = StartCoroutine(AnimateTemperature());
    }
    
    public void StopBaking()
    {
        if (!isBaking) return;
        isBaking = false;
        if (bakingCoroutine != null) StopCoroutine(bakingCoroutine);
        SetStatus("Baking stopped!");
        Debug.Log($"=== BAKING STOPPED at {elapsedSeconds:F1}s ===");
    }
    
    public void ToggleOvenDoor()
    {
        if (isDoorAnimating) return;
        if (isBaking)
        {
            SetStatus("Can't open door while baking!");
            return;
        }
        
        isOvenOpen = !isOvenOpen;
        doorTargetRotation = Quaternion.Euler(isOvenOpen ? doorOpenAngle : doorCloseAngle, 0, 0);
        isDoorAnimating = true;
        
        SetStatus(isOvenOpen ? "Oven door open" : "Oven door closed");
        if (debugMode) Debug.Log($"Oven door: {(isOvenOpen ? "Open" : "Closed")}");
    }
    
    public void PlaceDoughInOven(GameObject dough)
    {
        if (isBaking)
        {
            SetStatus("Can't place dough while baking!");
            return;
        }
        
        if (bakingTray != null)
        {
            foreach (Transform child in bakingTray.transform)
            {
                Destroy(child.gameObject);
            }
            
            dough.transform.SetParent(bakingTray.transform);
            dough.transform.localPosition = Vector3.zero;
            dough.transform.localRotation = Quaternion.identity;
            dough.transform.localScale = new Vector3(0.45f, 0.35f, 0.45f);
            
            SetStatus("Dough placed in oven!");
            if (debugMode) Debug.Log("Dough placed in oven");
        }
    }
    
    // ============================================
    // BAKING COROUTINE - ACCURATE SYSTEM
    // ============================================
    
    private IEnumerator BakingProcess()
    {
        // Calculate how long this bake should take
        float temperatureFactor = (currentTemperature - minTemperature) / (maxTemperature - minTemperature);
        float adjustedTimeSeconds = currentBakeTimeSeconds / (1 + temperatureFactor * 0.5f);
        
        // Convert recipe times to seconds
        float recipeMinSeconds = ConvertMinutesToSeconds(recipeMinTimeMinutes);
        float recipeMaxSeconds = ConvertMinutesToSeconds(recipeMaxTimeMinutes);
        float recipeIdealSeconds = ConvertMinutesToSeconds(recipeIdealTimeMinutes);
        
        // Track progress
        float progress = 0f;
        
        Debug.Log($"=== BAKING CALCULATIONS ===");
        Debug.Log($"Your Temp: {currentTemperature}°C (Recipe: {recipeIdealTemp}°C)");
        Debug.Log($"Your Time: {currentBakeTimeMinutes} min (Recipe: {recipeIdealTimeMinutes} min)");
        Debug.Log($"Adjusted time: {adjustedTimeSeconds:F1}s");
        Debug.Log($"Recipe range: {recipeMinSeconds:F1}s - {recipeMaxSeconds:F1}s");
        Debug.Log($"===========================");
        
        // ============================================
        // BAKING LOOP
        // ============================================
        
        while (elapsedSeconds < adjustedTimeSeconds && isBaking)
        {
            elapsedSeconds += Time.deltaTime;
            progress = elapsedSeconds / adjustedTimeSeconds;
            
            // Update visuals based on progress
            UpdateBakingVisuals(progress, elapsedSeconds, adjustedTimeSeconds);
            OnBakingProgress?.Invoke(progress);
            
            // Update status with time remaining
            float timeLeftSeconds = adjustedTimeSeconds - elapsedSeconds;
            float timeLeftMinutes = ConvertSecondsToMinutes(timeLeftSeconds);
            
            int minutes = Mathf.FloorToInt(timeLeftMinutes);
            int seconds = Mathf.FloorToInt(timeLeftSeconds % 60);
            SetStatus($"Baking... {minutes}m {seconds}s remaining");
            
            // Update time display
            if (timeDisplay != null)
            {
                float remainingMinutes = ConvertSecondsToMinutes(timeLeftSeconds);
                timeDisplay.text = $"{remainingMinutes:F1}m";
            }
            
            // Tick sound every 2 seconds
            if (Mathf.FloorToInt(elapsedSeconds) % 2 == 0 && elapsedSeconds > 0)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(ovenTickSFX, 0.3f);
            }
            
            yield return null;
        }
        
        // ============================================
        // BAKING COMPLETE - Determine Result
        // ============================================
        
        if (isBaking)
        {
            isBaking = false;
            bakingCompleted = true;
            
            // Determine the result based on temperature and time
            string result = DetermineBakeResult();
            float score = CalculateScore(result);
            
            // Play appropriate sound
            if (result == "burnt" && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(ovenBurntSFX);
            else if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(ovenDoneSFX);
            
            // Set status
            string statusMessage = GetResultMessage(result, score);
            SetStatus(statusMessage);
            
            // Update visuals
            UpdateFinalVisuals(result);
            
            // Spawn baked item
            SpawnBakedItem(result);
            
            // Save result
            SaveBakeResult(result, score);
            
            // Fire event
            OnBakingComplete?.Invoke(result, score);
            
            Debug.Log($"=== BAKING COMPLETE ===");
            Debug.Log($"Result: {result}");
            Debug.Log($"Score: {score:F0}%");
            Debug.Log($"Your Temp: {currentTemperature}°C (Recipe: {recipeIdealTemp}°C)");
            Debug.Log($"Your Time: {currentBakeTimeMinutes} min (Recipe: {recipeIdealTimeMinutes} min)");
            Debug.Log($"========================");
        }
        
        bakingCoroutine = null;
        temperatureAnimationCoroutine = null;
    }
    
    // ============================================
    // RESULT DETERMINATION
    // ============================================
    
    private string DetermineBakeResult()
    {
        // Check temperature first
        bool tempTooLow = currentTemperature < recipeMinTemp;
        bool tempTooHigh = currentTemperature > recipeMaxTemp;
        bool tempPerfect = !tempTooLow && !tempTooHigh;
        
        // Check time
        bool timeTooShort = currentBakeTimeMinutes < recipeMinTimeMinutes;
        bool timeTooLong = currentBakeTimeMinutes > recipeMaxTimeMinutes;
        bool timePerfect = !timeTooShort && !timeTooLong;
        
        // Determine result
        if (tempPerfect && timePerfect)
        {
            return "perfect";
        }
        else if (tempPerfect && timeTooLong)
        {
            // Perfect temp but too long = OVERCOOKED
            float overage = currentBakeTimeMinutes - recipeMaxTimeMinutes;
            if (overage > 10f) return "burnt";
            return "overcooked";
        }
        else if (tempPerfect && timeTooShort)
        {
            // Perfect temp but too short = UNDERCOOKED
            return "undercooked";
        }
        else if (tempTooHigh && timePerfect)
        {
            // Too hot but correct time = OVERCOOKED
            float overage = currentTemperature - recipeMaxTemp;
            if (overage > 30f) return "burnt";
            return "overcooked";
        }
        else if (tempTooHigh && timeTooLong)
        {
            // Too hot AND too long = BURNT
            return "burnt";
        }
        else if (tempTooHigh && timeTooShort)
        {
            // Too hot but too short = OVERCOOKED
            return "overcooked";
        }
        else if (tempTooLow && timePerfect)
        {
            // Too cold but correct time = UNDERCOOKED
            return "undercooked";
        }
        else if (tempTooLow && timeTooShort)
        {
            // Too cold AND too short = UNDERCOOKED
            return "undercooked";
        }
        else if (tempTooLow && timeTooLong)
        {
            // Too cold but too long = AVERAGE (might still cook through)
            return "average";
        }
        else
        {
            return "average";
        }
    }
    
    private float CalculateScore(string result)
    {
        float baseScore = 0f;
        float penalty = 0f;
        
        switch (result)
        {
            case "perfect":
                baseScore = perfectScore;
                break;
            case "average":
                baseScore = goodScore;
                break;
            case "undercooked":
                baseScore = undercookedScore;
                break;
            case "overcooked":
                baseScore = overcookedScore;
                break;
            case "burnt":
                baseScore = burntScore;
                break;
            default:
                baseScore = 50f;
                break;
        }
        
        // Calculate penalties for how far off the player was
        float tempDiff = Mathf.Abs(currentTemperature - recipeIdealTemp);
        float tempPenalty = tempDiff / 50f * 20f; // Up to 20% penalty
        
        float timeDiff = Mathf.Abs(currentBakeTimeMinutes - recipeIdealTimeMinutes);
        float timePenalty = timeDiff / 30f * 20f; // Up to 20% penalty
        
        // Apply penalties
        float finalScore = baseScore - tempPenalty - timePenalty;
        finalScore = Mathf.Clamp(finalScore, 0f, 100f);
        
        return finalScore;
    }
    
    private string GetResultMessage(string result, float score)
    {
        switch (result)
        {
            case "perfect":
                return $"🌟 Perfect bake! Score: {score:F0}%";
            case "average":
                return $"👍 Good bake! Score: {score:F0}%";
            case "undercooked":
                return $"❄️ Undercooked! Need more time or heat. Score: {score:F0}%";
            case "overcooked":
                return $"🔥 Overcooked! Too hot or too long. Score: {score:F0}%";
            case "burnt":
                return $"💀 Burnt! Way too hot or too long. Score: {score:F0}%";
            default:
                return $"Baking complete! Score: {score:F0}%";
        }
    }
    
    // ============================================
    // VISUAL UPDATES
    // ============================================
    
    private void UpdateBakingVisuals(float progress, float elapsed, float total)
    {
        if (doughMaterial == null) return;
        
        // Determine if the player is on the right track
        float timeRatio = elapsed / total;
        float tempRatio = (currentTemperature - minTemperature) / (maxTemperature - minTemperature);
        
        Color targetColor;
        
        // Check if temperature is way off
        bool tempTooHigh = currentTemperature > recipeMaxTemp + 20f;
        bool tempTooLow = currentTemperature < recipeMinTemp - 20f;
        
        if (tempTooHigh)
        {
            // Dough getting burnt
            targetColor = Color.Lerp(bakingColor, burntColor, timeRatio);
        }
        else if (tempTooLow)
        {
            // Dough not cooking properly
            targetColor = Color.Lerp(rawColor, underdoneColor, timeRatio * 0.5f);
        }
        else if (timeRatio >= 1f)
        {
            targetColor = perfectColor;
        }
        else if (timeRatio >= 0.7f)
        {
            targetColor = Color.Lerp(bakingColor, perfectColor, (timeRatio - 0.7f) / 0.3f);
        }
        else if (timeRatio >= 0.3f)
        {
            targetColor = bakingColor;
        }
        else
        {
            targetColor = rawColor;
        }
        
        doughMaterial.color = Color.Lerp(doughMaterial.color, targetColor, Time.deltaTime * 2f);
    }
    
    private void UpdateFinalVisuals(string result)
    {
        if (doughMaterial == null) return;
        
        switch (result)
        {
            case "perfect":
                doughMaterial.color = perfectColor;
                break;
            case "undercooked":
                doughMaterial.color = underdoneColor;
                break;
            case "overcooked":
                doughMaterial.color = overdoneColor;
                break;
            case "burnt":
                doughMaterial.color = burntColor;
                break;
            default:
                doughMaterial.color = perfectColor;
                break;
        }
    }
    
    private void SpawnBakedItem(string result)
    {
        if (bakingTray == null) return;
        
        GameObject itemToSpawn = null;
        
        // Determine which prefab to spawn
        if (result == "burnt" && burntItemPrefab != null)
        {
            itemToSpawn = burntItemPrefab;
        }
        else if (bakedItemPrefab != null)
        {
            itemToSpawn = bakedItemPrefab;
        }
        
        if (itemToSpawn != null)
        {
            GameObject bakedItem = Instantiate(itemToSpawn, bakingTray.transform.position, Quaternion.identity);
            bakedItem.transform.SetParent(bakingTray.transform);
            bakedItem.transform.localPosition = Vector3.zero;
            bakedItem.transform.localScale = new Vector3(0.5f, 0.4f, 0.5f);
        }
    }
    
    private IEnumerator AnimateTemperature()
    {
        float targetTemp = currentTemperature;
        float currentDisplayTemp = 0f;
        
        while (isBaking)
        {
            currentDisplayTemp = Mathf.Lerp(currentDisplayTemp, targetTemp, Time.deltaTime * 3f);
            if (temperatureDisplay != null)
                temperatureDisplay.text = $"{Mathf.RoundToInt(currentDisplayTemp)}°C";
            yield return null;
        }
        
        if (temperatureDisplay != null)
            temperatureDisplay.text = $"{Mathf.RoundToInt(currentTemperature)}°C";
    }
    
    private void SaveBakeResult(string result, float score, GameObject bakedItemObj = null)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetBakingResult(result, score, bakedItemObj);
        }
    }
    
    private void UpdateDisplay()
    {
        if (temperatureDisplay != null)
            temperatureDisplay.text = $"{Mathf.RoundToInt(currentTemperature)}°C";
        
        if (timeDisplay != null)
        {
            if (currentBakeTimeMinutes % 1 == 0)
                timeDisplay.text = $"{Mathf.RoundToInt(currentBakeTimeMinutes)}m";
            else
                timeDisplay.text = $"{currentBakeTimeMinutes:F1}m";
        }
    }
    
    private void SetStatus(string message)
    {
        if (statusDisplay != null)
            statusDisplay.text = message;
        if (debugMode) Debug.Log($"Status: {message}");
    }
    
    public float GetProgress()
    {
        if (currentBakeTimeSeconds <= 0) return 0;
        return Mathf.Clamp01(elapsedSeconds / currentBakeTimeSeconds);
    }
    
    public void SetBakeTimeMinutes(float minutes)
    {
        if (isBaking) return;
        currentBakeTimeMinutes = Mathf.Clamp(minutes, minBakeTimeMinutes, maxBakeTimeMinutes);
        currentBakeTimeSeconds = ConvertMinutesToSeconds(currentBakeTimeMinutes);
        UpdateDisplay();
    }
    
    public float GetRemainingTimeMinutes()
    {
        if (!isBaking) return currentBakeTimeMinutes;
        float remainingSeconds = Mathf.Max(0, currentBakeTimeSeconds - elapsedSeconds);
        return ConvertSecondsToMinutes(remainingSeconds);
    }
    
    private void OnDestroy()
    {
        if (bakingCoroutine != null) StopCoroutine(bakingCoroutine);
        if (temperatureAnimationCoroutine != null) StopCoroutine(temperatureAnimationCoroutine);
    }
}