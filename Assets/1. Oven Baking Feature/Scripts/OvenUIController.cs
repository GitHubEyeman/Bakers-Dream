using UnityEngine;
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
    
    private void Start()
    {
        if (ovenController == null)
        {
            ovenController = FindObjectOfType<OvenController>();
            if (ovenController == null)
            {
                Debug.LogError("No OvenController found in scene!");
                return;
            }
        }
        
        // Subscribe to oven events
        ovenController.OnBakingProgress += UpdateProgress;
        ovenController.OnBakingComplete += OnBakingComplete;
        ovenController.OnBakingBurnt += OnBakingBurnt;
        
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
        // Toggle oven panel with O key
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleOvenPanel();
        }
    }
    
    public void ToggleOvenPanel()
    {
        if (ovenPanel != null)
        {
            ovenPanel.SetActive(!ovenPanel.activeSelf);
            UpdateUI();
        }
    }
    
    private void UpdateUI()
    {
        if (ovenController == null) return;
        
        // Update status text
        if (statusText != null && ovenController.IsBaking)
        {
            float timeLeft = ovenController.TotalBakeTime - ovenController.ElapsedTime;
            statusText.text = $"Baking... {Mathf.Max(0, timeLeft):F1}s";
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
            else if (progress > 0.8f)
                progressFillImage.color = bakingColor;
            else
                progressFillImage.color = Color.white;
        }
        
        UpdateUI();
    }
    
    private void OnBakingComplete()
    {
        if (progressText != null)
            progressText.text = "DONE!";
        if (statusText != null)
            statusText.text = "Perfectly baked!";
        if (progressFillImage != null)
            progressFillImage.color = doneColor;
    }
    
    private void OnBakingBurnt()
    {
        if (progressText != null)
            progressText.text = "BURNT!";
        if (statusText != null)
            statusText.text = "Burnt! Too long in oven!";
        if (progressFillImage != null)
            progressFillImage.color = burntColor;
    }
    
    private void ClearOven()
    {
        if (ovenController.IsBaking)
            ovenController.StopBaking();
        
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
            ovenController.OnBakingBurnt -= OnBakingBurnt;
        }
    }
}