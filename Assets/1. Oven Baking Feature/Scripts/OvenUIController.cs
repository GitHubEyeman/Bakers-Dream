using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class OvenUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OvenController ovenController;
    [SerializeField] private GameObject ovenPanel;
    [SerializeField] private Button startBakeButton;
    [SerializeField] private Button stopBakeButton;
    [SerializeField] private Button doorToggleButton;
    [SerializeField] private Button clearButton;
    
    [Header("Progress Display")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image progressFillImage;
    
    [Header("Colors")]
    [SerializeField] private Color bakingColor = Color.yellow;
    [SerializeField] private Color doneColor = Color.green;
    [SerializeField] private Color burntColor = Color.red;
    
    private bool panelVisible = false;

    private void Start()
    {
        if (ovenController == null)
        {
            ovenController = FindFirstObjectByType<OvenController>();
            if (ovenController == null)
            {
                Debug.LogError("No OvenController found in scene!");
                return;
            }
        }
        
        // Subscribe to oven events
        ovenController.OnBakingProgress += UpdateProgress;
        ovenController.OnBakingComplete += OnBakingComplete;
        
        // Setup UI buttons
        if (startBakeButton != null)
            startBakeButton.onClick.AddListener(() => ovenController.StartBaking());
        if (stopBakeButton != null)
            stopBakeButton.onClick.AddListener(() => ovenController.StopBaking());
        if (doorToggleButton != null)
            doorToggleButton.onClick.AddListener(() => ovenController.ToggleOvenDoor());
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearOven);
        
        // Initial UI state
        UpdateUI();
        
        if (ovenPanel != null)
            ovenPanel.SetActive(false);
    }
    
    private void Update()
    {
        // Toggle oven panel with O key using new Input System
        if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
        {
            ToggleOvenPanel();
        }
        
        // Update status in real-time
        if (ovenController != null && statusText != null)
        {
            // Status is updated by the oven controller
        }
    }
    
    public void ToggleOvenPanel()
    {
        if (ovenPanel != null)
        {
            panelVisible = !panelVisible;
            ovenPanel.SetActive(panelVisible);
            UpdateUI();
        }
    }
    
    private void UpdateUI()
    {
        if (ovenController == null) return;
        
        // Update status text from oven
        if (statusText != null && ovenController.IsBaking)
        {
            // Use TotalBakeTimeSeconds and ElapsedTime
            float timeLeft = ovenController.TotalBakeTimeSeconds - ovenController.ElapsedTime;
            float timeLeftMinutes = timeLeft / 60f; // Convert to minutes for display
            
            if (timeLeftMinutes >= 1f)
            {
                int minutes = Mathf.FloorToInt(timeLeftMinutes);
                int seconds = Mathf.FloorToInt(timeLeft % 60);
                statusText.text = $"Baking... {minutes}m {seconds}s";
            }
            else
            {
                statusText.text = $"Baking... {Mathf.CeilToInt(timeLeft)}s";
            }
        }
        else if (statusText != null && ovenController.IsBakingCompleted)
        {
            statusText.text = "Baking complete!";
        }
        else if (statusText != null)
        {
            statusText.text = "Ready to bake!";
        }
    }
    
    private void UpdateProgress(float progress)
    {
        if (progressSlider != null)
        {
            progressSlider.value = progress;
        }
        
        if (progressText != null)
        {
            progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }
        
        if (progressFillImage != null)
        {
            if (progress >= 1f)
                progressFillImage.color = doneColor;
            else if (progress > 0.7f)
                progressFillImage.color = bakingColor;
            else
                progressFillImage.color = Color.white;
        }
        
        UpdateUI();
    }
    
    private void OnBakingComplete(string result, float score)
    {
        if (progressText != null)
            progressText.text = "DONE!";
        if (statusText != null)
            statusText.text = $"Baking complete! Score: {score:F0}%";
        if (progressFillImage != null)
        {
            if (result == "perfect")
                progressFillImage.color = doneColor;
            else if (result == "burnt")
                progressFillImage.color = burntColor;
            else
                progressFillImage.color = bakingColor;
        }
    }
    
    private void ClearOven()
    {
        if (ovenController != null)
        {
            if (ovenController.IsBaking)
                ovenController.StopBaking();
        }
        
        // Reset progress
        if (progressSlider != null)
            progressSlider.value = 0f;
        if (progressText != null)
            progressText.text = "0%";
        if (progressFillImage != null)
            progressFillImage.color = Color.white;
        if (statusText != null)
            statusText.text = "Cleared";
    }
    
    private void OnDestroy()
    {
        if (ovenController != null)
        {
            ovenController.OnBakingProgress -= UpdateProgress;
            ovenController.OnBakingComplete -= OnBakingComplete;
        }
    }
}