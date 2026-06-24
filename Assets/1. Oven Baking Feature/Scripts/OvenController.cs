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
    
    [Header("Timer Settings")]
    [SerializeField] private float minBakeTime = 10f;
    [SerializeField] private float maxBakeTime = 60f;
    [SerializeField] private float defaultBakeTime = 30f;
    [SerializeField] private float timeStep = 1f;
    
    [Header("UI References")]
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI temperatureDisplay;
    [SerializeField] private TextMeshProUGUI timeDisplay;
    [SerializeField] private TextMeshProUGUI statusDisplay;
    [SerializeField] private GameObject ovenDoor;
    [SerializeField] private GameObject bakingTray;
    [SerializeField] private GameObject bakedItemPrefab;
    
    [Header("Door Settings")]
    [SerializeField] private float doorOpenAngle = -110f;
    [SerializeField] private float doorCloseAngle = 0f;
    [SerializeField] private float doorSpeed = 3f;
    [SerializeField] private GameObject doorPivot;
    
    [Header("Baking Visuals")]
    [SerializeField] private Material doughMaterial;
    [SerializeField] private Color rawColor = Color.white;
    [SerializeField] private Color bakingColor = Color.yellow;
    [SerializeField] private Color doneColor = new Color(0.85f, 0.65f, 0.13f);
    [SerializeField] private Color burntColor = Color.black;
    
    [Header("Audio")]
    [SerializeField] private string ovenStartSFX = "ovenStart";
    [SerializeField] private string ovenTickSFX = "ovenTick";
    [SerializeField] private string ovenDoneSFX = "ovenDone";
    [SerializeField] private string ovenBurntSFX = "ovenBurnt";
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    // Private Variables
    private float currentTemperature;
    private float currentBakeTime;
    private float elapsedTime = 0f;
    private bool isBaking = false;
    private bool isOvenOpen = false;
    private Coroutine bakingCoroutine;
    private Coroutine temperatureAnimationCoroutine;
    
    // Door Animation Variables
    private bool isDoorAnimating = false;
    private Quaternion doorTargetRotation;
    
    // Events
    public event Action<float> OnBakingProgress;
    public event Action OnBakingComplete;
    public event Action OnBakingBurnt;
    
    // Properties
    public bool IsBaking => isBaking;
    public float CurrentTemperature => currentTemperature;
    public float ElapsedTime => elapsedTime;
    public float TotalBakeTime => currentBakeTime;
    
    private void Start()
    {
        // Initialize with default values
        currentTemperature = defaultTemperature;
        currentBakeTime = defaultBakeTime;
        
        // Setup UI
        if (temperatureSlider != null)
        {
            temperatureSlider.minValue = minTemperature;
            temperatureSlider.maxValue = maxTemperature;
            temperatureSlider.value = currentTemperature;
            temperatureSlider.onValueChanged.AddListener(OnTemperatureChanged);
        }
        
        if (timeSlider != null)
        {
            timeSlider.minValue = minBakeTime;
            timeSlider.maxValue = maxBakeTime;
            timeSlider.value = currentBakeTime;
            timeSlider.onValueChanged.AddListener(OnTimeChanged);
        }
        
        UpdateDisplay();
        SetStatus("Ready to bake!");
        
        // Setup door pivot
        if (doorPivot == null && ovenDoor != null)
        {
            doorPivot = ovenDoor.transform.Find("DoorPivot")?.gameObject;
            if (doorPivot == null)
            {
                doorPivot = ovenDoor;
            }
        }
        
        // Set initial dough color
        if (doughMaterial != null)
        {
            doughMaterial.color = rawColor;
        }
        
        // Make sure door is closed
        if (doorPivot != null)
        {
            doorPivot.transform.localRotation = Quaternion.Euler(doorCloseAngle, 0, 0);
        }
        isOvenOpen = false;
    }
    
    private void Update()
    {
        // Keyboard shortcuts using new Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                ToggleBaking();
            }
            if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                ToggleOvenDoor();
            }
        }
        
        // Handle door animation smoothly
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
    
    #region UI Event Handlers
    
    public void OnTemperatureChanged(float value)
    {
        if (isBaking) return; // Can't change temperature while baking
        currentTemperature = Mathf.Round(value / temperatureStep) * temperatureStep;
        UpdateDisplay();
        if (debugMode) Debug.Log($"Temperature set to: {currentTemperature}°C");
    }
    
    public void OnTimeChanged(float value)
    {
        if (isBaking) return; // Can't change time while baking
        currentBakeTime = Mathf.Round(value / timeStep) * timeStep;
        UpdateDisplay();
        if (debugMode) Debug.Log($"Time set to: {currentBakeTime} seconds");
    }
    
    #endregion
    
    #region Public Methods
    
    public void ToggleBaking()
    {
        if (!isBaking)
        {
            StartBaking();
        }
        else
        {
            StopBaking();
        }
    }
    
    public void StartBaking()
    {
        if (isBaking) return;
        if (isOvenOpen)
        {
            SetStatus("Close the oven door first!");
            return;
        }
        
        // Reset elapsed time
        elapsedTime = 0f;
        isBaking = true;
        
        // Play sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(ovenStartSFX);
        
        // Start baking coroutine
        if (bakingCoroutine != null)
            StopCoroutine(bakingCoroutine);
        bakingCoroutine = StartCoroutine(BakingProcess());
        
        SetStatus("Baking...");
        if (debugMode) Debug.Log("Baking started!");
        
        // Animate temperature color
        if (temperatureAnimationCoroutine != null)
            StopCoroutine(temperatureAnimationCoroutine);
        temperatureAnimationCoroutine = StartCoroutine(AnimateTemperature());
    }
    
    public void StopBaking()
    {
        if (!isBaking) return;
        
        isBaking = false;
        
        if (bakingCoroutine != null)
        {
            StopCoroutine(bakingCoroutine);
            bakingCoroutine = null;
        }
        
        SetStatus("Baking stopped!");
        if (debugMode) Debug.Log("Baking stopped!");
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
        
        doorTargetRotation = Quaternion.Euler(
            isOvenOpen ? doorOpenAngle : doorCloseAngle,
            0,
            0
        );
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
            // Clear previous dough
            foreach (Transform child in bakingTray.transform)
            {
                Destroy(child.gameObject);
            }
            
            // Place dough on tray
            dough.transform.SetParent(bakingTray.transform);
            dough.transform.localPosition = Vector3.zero;
            dough.transform.localRotation = Quaternion.identity;
            
            SetStatus("Dough placed in oven!");
            if (debugMode) Debug.Log("Dough placed in oven");
        }
    }
    
    #endregion
    
    #region Baking Coroutine
    
    private IEnumerator BakingProcess()
    {
        float progress = 0f;
        float temperatureFactor = (currentTemperature - minTemperature) / (maxTemperature - minTemperature);
        float actualBakeTime = currentBakeTime / (1 + temperatureFactor * 0.5f);
        float burnThreshold = actualBakeTime * 1.2f;
        
        while (elapsedTime < burnThreshold && isBaking)
        {
            elapsedTime += Time.deltaTime;
            progress = Mathf.Clamp01(elapsedTime / actualBakeTime);
            
            UpdateBakingVisuals(progress);
            OnBakingProgress?.Invoke(progress);
            
            int timeLeft = Mathf.CeilToInt(actualBakeTime - elapsedTime);
            if (timeLeft > 0)
            {
                SetStatus($"Baking... {timeLeft}s remaining");
            }
            else if (elapsedTime < burnThreshold)
            {
                SetStatus("DONE! Take it out!");
                if (elapsedTime - Time.deltaTime < actualBakeTime)
                {
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlaySFX(ovenDoneSFX);
                }
            }
            
            if (timeDisplay != null)
            {
                timeDisplay.text = $"{Mathf.Max(0, actualBakeTime - elapsedTime):F1}s";
            }
            
            if (Mathf.FloorToInt(elapsedTime) % 2 == 0 && elapsedTime > 0)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(ovenTickSFX, 0.3f);
            }
            
            yield return null;
        }
        
        if (isBaking)
        {
            isBaking = false;
            
            if (elapsedTime >= burnThreshold)
            {
                SetStatus("BURNT! Too long in the oven!");
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(ovenBurntSFX);
                OnBakingBurnt?.Invoke();
                UpdateBakingVisuals(1.5f);
                if (debugMode) Debug.Log("Bread burnt!");
            }
            else if (progress >= 1f)
            {
                SetStatus("Perfectly baked!");
                OnBakingComplete?.Invoke();
                UpdateBakingVisuals(1f);
                
                if (bakedItemPrefab != null && bakingTray != null)
                {
                    GameObject bakedItem = Instantiate(bakedItemPrefab, bakingTray.transform.position, Quaternion.identity);
                    bakedItem.transform.SetParent(bakingTray.transform);
                    bakedItem.transform.localPosition = Vector3.zero;
                }
                
                if (debugMode) Debug.Log("Baking complete!");
            }
        }
        
        bakingCoroutine = null;
        temperatureAnimationCoroutine = null;
    }
    
    #endregion
    
    #region Visual Updates
    
    private void UpdateBakingVisuals(float progress)
    {
        if (doughMaterial == null) return;
        
        Color targetColor;
        if (progress >= 1.2f)
        {
            targetColor = burntColor;
        }
        else if (progress >= 0.9f)
        {
            targetColor = doneColor;
        }
        else if (progress >= 0.3f)
        {
            targetColor = bakingColor;
        }
        else
        {
            targetColor = rawColor;
        }
        
        doughMaterial.color = Color.Lerp(doughMaterial.color, targetColor, Time.deltaTime * 2f);
    }
    
    private IEnumerator AnimateTemperature()
    {
        float targetTemp = currentTemperature;
        float currentDisplayTemp = 0f;
        
        while (isBaking)
        {
            currentDisplayTemp = Mathf.Lerp(currentDisplayTemp, targetTemp, Time.deltaTime * 3f);
            if (temperatureDisplay != null)
            {
                temperatureDisplay.text = $"{Mathf.RoundToInt(currentDisplayTemp)}°C";
            }
            yield return null;
        }
        
        if (temperatureDisplay != null)
        {
            temperatureDisplay.text = $"{Mathf.RoundToInt(currentTemperature)}°C";
        }
    }
    
    #endregion
    
    #region UI Updates
    
    private void UpdateDisplay()
    {
        if (temperatureDisplay != null)
        {
            temperatureDisplay.text = $"{Mathf.RoundToInt(currentTemperature)}°C";
        }
        
        if (timeDisplay != null)
        {
            timeDisplay.text = $"{Mathf.RoundToInt(currentBakeTime)}s";
        }
    }
    
    private void SetStatus(string message)
    {
        if (statusDisplay != null)
        {
            statusDisplay.text = message;
        }
        if (debugMode) Debug.Log($"Oven Status: {message}");
    }
    
    #endregion
    
    #region Recipe Integration
    
    public void SetOvenFromRecipe(RecipeData recipe)
    {
        OvenRecipeData ovenRecipe = recipe as OvenRecipeData;
        if (ovenRecipe != null)
        {
            float targetTemp = (ovenRecipe.minTimer + ovenRecipe.maxTimer) / 2f;
            targetTemp = Mathf.Clamp(targetTemp, minTemperature, maxTemperature);
            currentTemperature = Mathf.Round(targetTemp / temperatureStep) * temperatureStep;
            
            float targetTime = (ovenRecipe.minTimer + ovenRecipe.maxTimer) / 2f;
            targetTime = Mathf.Clamp(targetTime, minBakeTime, maxBakeTime);
            currentBakeTime = Mathf.Round(targetTime / timeStep) * timeStep;
            
            if (temperatureSlider != null)
                temperatureSlider.value = currentTemperature;
            if (timeSlider != null)
                timeSlider.value = currentBakeTime;
            
            UpdateDisplay();
            SetStatus($"Recipe set: {currentTemperature}°C for {currentBakeTime}s");
            
            if (debugMode) Debug.Log($"Oven set from recipe: {recipe.name}");
        }
        else
        {
            if (debugMode) Debug.Log($"No oven-specific settings found for recipe: {recipe.name}");
        }
    }
    
    #endregion
    
    #region Getters
    
    public float GetProgress()
    {
        if (currentBakeTime <= 0) return 0;
        return Mathf.Clamp01(elapsedTime / currentBakeTime);
    }
    
    public bool IsDone()
    {
        return elapsedTime >= currentBakeTime && isBaking;
    }
    
    public bool IsBurnt()
    {
        return elapsedTime > currentBakeTime * 1.2f && isBaking;
    }
    
    #endregion
}