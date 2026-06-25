using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BakingSceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OvenController ovenController;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject ovenPanel;
    [SerializeField] private GameObject doughPrefab;
    
    [Header("Result Display")]
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI resultScoreText;
    [SerializeField] private TextMeshProUGUI resultDetailText;
    [SerializeField] private TextMeshProUGUI resultFeedbackText;
    [SerializeField] private Image resultImage;
    [SerializeField] private Image resultBackgroundImage;
    
    [Header("Result Sprites")]
    [SerializeField] private Sprite perfectSprite;
    [SerializeField] private Sprite overcookedSprite;
    [SerializeField] private Sprite undercookedSprite;
    [SerializeField] private Sprite burntSprite;
    
    [Header("Result Colors")]
    [SerializeField] private Color perfectColor = new Color(1f, 0.84f, 0f);
    [SerializeField] private Color overcookedColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color undercookedColor = new Color(0.3f, 0.7f, 1f);
    [SerializeField] private Color burntColor = new Color(0.8f, 0.1f, 0.1f);
    
    [Header("Scene Names")]
    [SerializeField] private string nextSceneName = "DecorationScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    
    private bool bakingCompleted = false;
    private string bakingResult = "";
    private float bakingScore = 0f;
    private GameObject currentDough;
    private bool wasPanelVisible = false;

    private void Start()
    {
        // Find references if not assigned
        if (ovenController == null)
            ovenController = FindFirstObjectByType<OvenController>();
        
        // Find oven panel if not assigned
        if (ovenPanel == null)
        {
            OvenUIController uiController = FindFirstObjectByType<OvenUIController>();
            if (uiController != null)
            {
                Canvas canvas = uiController.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    ovenPanel = canvas.transform.Find("OvenPanel")?.gameObject;
                }
            }
        }
        
        // Subscribe to events
        if (ovenController != null)
        {
            ovenController.OnBakingComplete += OnBakingComplete;
        }
        
        // Setup UI
        if (resultPanel != null)
            resultPanel.SetActive(false);
        
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(GoToNextScene);
            nextButton.gameObject.SetActive(false);
        }
        
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryBaking);
            retryButton.gameObject.SetActive(false);
        }
        
        // Create test dough if none exists
        CheckAndCreateDough();
        
        Debug.Log("BakingSceneManager initialized!");
    }

    private void CheckAndCreateDough()
    {
        if (GameManager.Instance != null && GameManager.Instance.hasDoughReady)
        {
            Debug.Log("Dough found in GameManager!");
            return;
        }
        
        if (doughPrefab != null && ovenController != null)
        {
            GameObject dough = Instantiate(doughPrefab);
            currentDough = dough;
            ovenController.PlaceDoughInOven(dough);
            Debug.Log("Created test dough.");
        }
        else
        {
            Debug.LogWarning("No dough prefab assigned!");
        }
    }

    private void OnBakingComplete(string result, float score)
    {
        bakingCompleted = true;
        bakingResult = result;
        bakingScore = score;
        
        ShowResult(result, score);
    }

    private void ShowResult(string result, float score)
    {
        // Hide oven panel when result shows
        if (ovenPanel != null)
        {
            wasPanelVisible = ovenPanel.activeSelf;
            if (wasPanelVisible)
            {
                ovenPanel.SetActive(false);
                Debug.Log("Oven panel hidden - result showing");
            }
        }
        
        // Show result panel
        if (resultPanel != null)
            resultPanel.SetActive(true);
        
        // Show buttons
        if (nextButton != null)
            nextButton.gameObject.SetActive(true);
        if (retryButton != null)
            retryButton.gameObject.SetActive(true);
        
        // Determine result data
        string title = "";
        string detail = "";
        string feedback = "";
        Color titleColor = Color.white;
        Color bgColor = Color.black;
        Sprite resultSprite = null;
        
        switch (result)
        {
            case "perfect":
                title = "🌟 Perfect Bake! 🌟";
                detail = "You followed the recipe perfectly!";
                feedback = "✓ Correct temperature ✓ Correct time\n✓ Perfect texture and color!";
                titleColor = perfectColor;
                bgColor = new Color(0.2f, 0.3f, 0.1f, 0.9f);
                resultSprite = perfectSprite;
                break;
                
            case "overcooked":
                title = "🔥 Overcooked!";
                detail = "Too hot or baked too long.";
                feedback = "✗ Temperature too high\n✗ Baked for too long\nTry lowering the temperature or reducing time.";
                titleColor = overcookedColor;
                bgColor = new Color(0.4f, 0.2f, 0f, 0.9f);
                resultSprite = overcookedSprite;
                break;
                
            case "undercooked":
                title = "❄️ Undercooked!";
                detail = "Not hot enough or baked too short.";
                feedback = "✗ Temperature too low\n✗ Baked for too short\nTry increasing the temperature or adding more time.";
                titleColor = undercookedColor;
                bgColor = new Color(0f, 0.2f, 0.4f, 0.9f);
                resultSprite = undercookedSprite;
                break;
                
            case "burnt":
                title = "💀 Burnt!";
                detail = "Way too hot and baked too long!";
                feedback = "✗ Temperature WAY too high\n✗ Baked WAY too long\nThe dough is completely ruined!";
                titleColor = burntColor;
                bgColor = new Color(0.4f, 0f, 0f, 0.9f);
                resultSprite = burntSprite;
                break;
                
            case "average":
                title = "👍 Average Bake";
                detail = "Close, but not quite perfect.";
                feedback = "Try adjusting your temperature or time\nCloser to the recipe next time!";
                titleColor = new Color(0.5f, 0.8f, 0.5f);
                bgColor = new Color(0.2f, 0.2f, 0.1f, 0.9f);
                resultSprite = perfectSprite; // Use perfect sprite as fallback
                break;
                
            default:
                title = "Baking Complete!";
                detail = "Baking is done.";
                feedback = "Check your settings and try again!";
                titleColor = Color.white;
                bgColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
                resultSprite = perfectSprite;
                break;
        }
        
        // Apply to UI
        if (resultTitleText != null)
        {
            resultTitleText.text = title;
            resultTitleText.color = titleColor;
        }
        
        if (resultScoreText != null)
            resultScoreText.text = $"Score: {score:F0}%";
        
        if (resultDetailText != null)
            resultDetailText.text = detail;
        
        if (resultFeedbackText != null)
            resultFeedbackText.text = feedback;
        
        if (resultImage != null && resultSprite != null)
            resultImage.sprite = resultSprite;
        
        if (resultBackgroundImage != null)
            resultBackgroundImage.color = bgColor;
    }

    public void GoToNextScene()
    {
        if (!bakingCompleted)
        {
            Debug.Log("Please wait for baking to complete!");
            return;
        }
        
        // Store final result in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.bakingResult = bakingResult;
            GameManager.Instance.bakingScore = bakingScore;
        }
        
        // Load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name not set!");
        }
    }

    public void RetryBaking()
    {
        // Reset oven
        if (ovenController != null)
        {
            ovenController.StopBaking();
        }
        
        // Hide result panel
        if (resultPanel != null)
            resultPanel.SetActive(false);
        
        // Hide result buttons
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
        if (retryButton != null)
            retryButton.gameObject.SetActive(false);
        
        // Show oven panel again if it was visible before
        if (ovenPanel != null && wasPanelVisible)
        {
            ovenPanel.SetActive(true);
            Debug.Log("Oven panel shown again - retry");
        }
        
        bakingCompleted = false;
        
        // Reset dough
        if (currentDough != null && ovenController != null)
        {
            ovenController.PlaceDoughInOven(currentDough);
        }
        
        // Reset status
        if (ovenController != null)
        {
            ovenController.OnBakingComplete += OnBakingComplete;
        }
        
        // Reset progress UI
        OvenUIController uiController = FindFirstObjectByType<OvenUIController>();
        if (uiController != null)
        {
            Slider progressSlider = uiController.GetComponentInChildren<Slider>();
            if (progressSlider != null)
                progressSlider.value = 0f;
        }
        
        Debug.Log("Retry baking! Oven panel restored.");
    }

    public void GoToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        if (ovenController != null)
        {
            ovenController.OnBakingComplete -= OnBakingComplete;
        }
    }
}